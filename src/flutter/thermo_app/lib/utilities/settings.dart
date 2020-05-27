import 'package:shared_preferences/shared_preferences.dart';

class Settings {
  SharedPreferences _preferences;

  Future initialize() async {
    if (_preferences == null){
      _preferences = await SharedPreferences.getInstance();
    }
  }

  String getSetting(String key){
    if (_preferences == null){
      throw Exception("設定が初期化されていません。");
    }
    return _preferences.getString(key);
  }

  Future setSetting(String key, String value) {
    if (_preferences == null){
      throw Exception("設定が初期化されていません。");
    }
    return _preferences.setString(key, value);
  }
}