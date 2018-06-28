'use strict';
import React, { Component } from 'react';
import {
  StyleSheet,
  Text,
  View
} from 'react-native';

import Camera from 'react-native-camera';

import ImageRecognizer from './ImageRecognizer';

export default class CameraApp extends Component {

  componentDidMount() {
    this.recognizer = new ImageRecognizer({
      model: require('./assets/model.pb'),
      labels: require('./assets/labels.txt'),
    });
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
      const result = await this.recognizer.recognize({
        image: data.path,
      });
      
      alert(JSON.stringify(result));
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
