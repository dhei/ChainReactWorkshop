import { compileModel, classifyTopValue } from "react-native-coreml";

export default class ImageRecognizer
{
  constructor(options)
  {
    compileModel(options).then((result) => {
        this.recognizer = result;
        alert(`picture file path: ${data}, model path: ${result}`);
      }
    ).catch((e) => {
      alert(e);
    });
  }

  async recognize(data)
  {
    // alert(`picture file path: ${data}, model path: ${this.recognizer}`);

    return await classifyTopValue(data, this.recognizer);
  }
}
