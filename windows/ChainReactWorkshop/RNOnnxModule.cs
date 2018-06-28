using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Web.Http;

namespace ChainReactWorkshop
{
    class RNOnnxModule : NativeModuleBase
    {
        private LearningModelPreview _model;

        public override string Name
        {
            get
            {
                return "RNOnnx";
            }
        }

        [ReactMethod]
        public async void load(string path, IPromise promise)
        {
            try
            {
                var storageFile = await GetFileAsync(path);
                _model = await LearningModelPreview.LoadModelFromStorageFileAsync(storageFile);
                promise.Resolve(null);
            }
            catch (Exception e)
            {
                promise.Reject(e);
            }
        }

        [ReactMethod]
        public async void evaluate(JObject data, string labelsPath, IPromise promise)
        {
            if (_model == null)
            {
                promise.Reject("INVALID_MODEL", "Run RNOnnx.load first");
                return;
            }

            VideoFrame frame;
            var binding = new LearningModelBindingPreview(_model);
            if (ContainsKey(data, "image"))
            {
                var file = await StorageFile.GetFileFromPathAsync(data.Value<string>("image"));
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var bmp = await decoder.GetSoftwareBitmapAsync();
                    frame = VideoFrame.CreateWithSoftwareBitmap(bmp);
                }
            }
            else
            {
                promise.Reject("INVALID_DATA", "Expected property 'image' to be set.");
                return;
            }

            using (frame)
            {
                var labels = await ReadLabelsAsync(labelsPath);
                var classLabels = new List<string>();
                var loss = new Dictionary<string, float>(labels.Length);
                foreach (var label in labels)
                {
                    loss.Add(label, float.NaN);
                }

                binding.Bind("data", frame);
                binding.Bind("classLabel", classLabels);
                binding.Bind("loss", loss);

                var result = await _model.EvaluateAsync(binding, Guid.NewGuid().ToString());

                promise.Resolve(new JObject
                {
                    { "classLabel", JArray.FromObject(classLabels) },
                    { "loss", JObject.FromObject(loss) },
                    { "result", JObject.FromObject(result) },
                });
            }
        }

        private static bool ContainsKey(IDictionary<string, JToken> map, string key)
        {
            return map.ContainsKey(key);
        }

        private static Task<StorageFile> GetFileAsync(string path)
        {
            if (path.StartsWith("http:") || path.StartsWith("https:"))
            {
                return DownloadAsync(path);
            }
            else if (path.StartsWith("ms-appx:") || path.StartsWith("ms-appdata:"))
            {
                return StorageFile.GetFileFromApplicationUriAsync(new Uri(path)).AsTask();
            }

            return StorageFile.GetFileFromPathAsync(path).AsTask();
        }   
        
        private static async Task<StorageFile> DownloadAsync(string path)
        {
            var httpClient = new HttpClient();
            var folder = ApplicationData.Current.TemporaryFolder;
            var file = await folder.CreateFileAsync("model.onnx", CreationCollisionOption.ReplaceExisting);
            using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            using (var response = await httpClient.GetAsync(new Uri(path)))
            {
                await response.Content.WriteToStreamAsync(outputStream);
            }

            return file;
        }

        private static async Task<string[]> ReadLabelsAsync(string path)
        {
            if (path.StartsWith("http:") || path.StartsWith("https:"))
            {
                var httpClient = new HttpClient();
                using (var response = await httpClient.GetAsync(new Uri(path)))
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            else if (path.StartsWith("ms-appx:") || path.StartsWith("ms-appdata:"))
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
                using (var stream = await file.OpenStreamForReadAsync())
                using (var reader = new StreamReader(stream))
                {
                    var content = await reader.ReadToEndAsync();
                    return content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            return await File.ReadAllLinesAsync(path);
        }
    }
}
