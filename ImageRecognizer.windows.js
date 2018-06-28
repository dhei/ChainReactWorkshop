import { 
  Image,
  NativeModules 
} from 'react-native';

export default class ImageRecognizer
{
  constructor(options)
  {
    const resolvedModel = Image.resolveAssetSource(options.model).uri;
    const model = resolvedModel || options.model;
    this.init = NativeModules.RNOnnx.load(model);
    const resolvedLabels = Image.resolveAssetSource(options.labels).uri;
    this.labels = resolvedLabels || options.labels;
  }

  async recognize(data)
  {
    await this.init;

    return await NativeModules.RNOnnx.evaluate(data, this.labels);
  }
}
