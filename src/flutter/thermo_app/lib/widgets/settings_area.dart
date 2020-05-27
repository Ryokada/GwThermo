import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:thermoapp/provideres/settings_provider.dart';

class SettingsArea extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    // TODO: implement build
    return Consumer<SettingsProvider>(
      builder: (_, provider, __){
        if (provider.isInitialised){
          return SettingsForm(provider);
        }
        else {
          return Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: <Widget>[
                Text("wait")
//                CircularProgressIndicator(),
              ],
            ),
          );
        }
      },
    );
  }
}

class SettingsForm extends StatelessWidget {
  final _formKey = GlobalKey<FormState>();
  final SettingsProvider _settings;

  String _inputId;
  String _inputUrl;

  SettingsForm(this._settings);

  @override
  Widget build(BuildContext context) {
    // TODO: implement build
    return Form(
        key: _formKey,
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 12.0, horizontal: 32),
          child: Column(
            children: <Widget>[
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: <Widget>[
                  Text("Device ID")
                ],
              ),
              TextFormField(
                keyboardType: TextInputType.text,
                initialValue: _settings.getDeviceId(),
                decoration: InputDecoration(
                  border: OutlineInputBorder(),
                  labelText: "Device ID",
                ),
                onSaved: (value) {
                  _inputId = value;
                },
                validator: (value) {
                  if (value.isEmpty) return '設定してください';
                  return null;
                },
              ),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: <Widget>[
                  Text("API URL"),
                ],
              ),
              TextFormField(
                  keyboardType: TextInputType.url,
                  initialValue: _settings.getApiURl(),
                  decoration: InputDecoration(
                    border: OutlineInputBorder(),
                    labelText: "API URL",
                  ),
                  onSaved: (value) {
                    _inputUrl = value;
                  },
                  validator: (value) {
                    if (value.isEmpty) return '設定してください';
                    return null;
                  }
              ),
              Center(
                child: RaisedButton(
                  color: Colors.blue,
                  textColor: Colors.white,
                  child: Text("保存"),
                  onPressed: () async {
                    if (_formKey.currentState.validate()){
                      _formKey.currentState.save();
                      final tasks = [_settings.setDeviceID(_inputId), _settings.setApiURL(_inputUrl)];
                      await Future.wait(tasks);

                      await showDialog<int>(
                        context: context,
                        barrierDismissible: false,
                        builder: (BuildContext context) {
                          return AlertDialog(
                            title: Text('保存完了'),
                            content: Text('保存しました。'),
                            actions: <Widget>[
                              FlatButton(
                                child: Text('OK'),
                                onPressed: () => Navigator.pop(context),
                              ),
                            ],
                          );
                        },
                      );
                    }
                  },
                ),
              )
            ],
          ),
        )
    );
  }
}