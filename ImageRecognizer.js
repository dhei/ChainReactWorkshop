import { TfImageRegonition } from 'react-native-tensorflow';

export default class ImageRecognizer
{
  constructor(options)
  {
    this.recognizer = new TfImageRegonition(options);
  }

  async recognize(data)
  {
    return await this.recognizer.recognize(data);
  }
}
