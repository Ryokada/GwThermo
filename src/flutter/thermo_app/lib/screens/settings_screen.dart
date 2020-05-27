import 'package:flutter/material.dart';
import 'package:thermoapp/widgets/settings_area.dart';

class SettingsScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
          title: Text('設定'),
          backgroundColor: Colors.grey,
        ),
        body: Center(
          child: SettingsArea(),
        )
    );
  }
}
