import 'package:flutter/material.dart';
import 'package:thermoapp/utilities/settings.dart';

class SettingsProvider with ChangeNotifier {
  final Settings _settings;
  bool _isInitialized = false;

  get isInitialised => _isInitialized;

  String getDeviceId() {
    return _settings.getSetting("device_id");
  }
  Future setDeviceID(String value) async{
    await _settings.setSetting("device_id", value);
    notifyListeners();
  }

  String getApiURl() {
    return _settings.getSetting("api_url");
  }
  Future setApiURL(String value) async{
    await _settings.setSetting("api_url", value);
    notifyListeners();
  }

  SettingsProvider(this._settings);

  Future initialise() async {
    await _settings.initialize();
    _isInitialized = true;
    notifyListeners();
  }
}