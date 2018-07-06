'use strict';
import React, { Component } from 'react';
import {
  Platform,
  StyleSheet,
  Text,
  View
} from 'react-native';

import Camera from 'react-native-camera';
import RNFS from 'react-native-fs';
import ImageRecognizer from './ImageRecognizer';

export default class CameraApp extends Component {

  componentDidMount() {
    // this.recognizer = new ImageRecognizer({
    //   model: Platform.select({
    //     windows: require('./assets/model.onnx'),
    //     android: require('./assets/model.pb'),
    //     ios: require('./assets/Model.mlmodel')
    //   }),
    //   labels: Platform.select({
    //     ios: undefined,
    //     default: require('./assets/labels.txt')
    //   })
    // });
    const options = Platform.select({
      android: {
        model: require('./assets/model.pb'),
        labels: require('./assets/labels.txt')
      },
      ios: RNFS.MainBundlePath + '/assets/21bec62da90e4a00a534e26b9fbc2800.mlmodel',
      windows: {
        model: require('./assets/model.onnx'),
        labels: require('./assets/labels.txt')
      }
    });
    this.recognizer = new ImageRecognizer(options);
  }

  render() {
    return (
      <View style={styles.container}>
        <Camera
          ref={(cam) => {
            this.camera = cam;
          }}
          style={styles.preview}
          aspect={Camera.constants.Aspect.fill}>
        </Camera>
        <Text style={styles.capture} onPress={this.takePicture.bind(this)}>[CAPTURE]</Text>
      </View>
    );
  }

  async takePicture() {
    const options = {};
    //options.location = ...
    try
    {
      const data = await this.camera.capture({metadata: options});
      const path = Platform.select({
        android: {
          image: data.path,
          inputName: 'Placeholder',
          outputName: 'loss',
        },
        ios: data.path,
      });
      const results = await this.recognizer.recognize(path);
      if (results.length > 0) {
        // alert(`Name: ${results[0].name} - Confidence: ${results[0].confidence}`);
        alert(JSON.stringify(results));
      }
    }
    catch (err)
    {
      alert(err);
    }
  }
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    flexDirection: 'row',
  },
  preview: {
    flex: 1,
    justifyContent: 'flex-end',
    alignItems: 'center'
  },
  capture: {
    position: 'absolute',
    bottom: 0,
    backgroundColor: '#fff',
    borderRadius: 5,
    color: '#FFF',
    padding: 10,
    margin: 40
  }
});
