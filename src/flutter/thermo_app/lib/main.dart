import 'dart:async';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:thermoapp/pages_names.dart';
import 'package:thermoapp/provideres/settings_provider.dart';
import 'package:thermoapp/provideres/thermo_provider.dart';
import 'package:thermoapp/repositories/http_api_client.dart';
import 'package:thermoapp/screens/home_screen.dart';
import 'package:thermoapp/screens/settings_screen.dart';
import 'package:thermoapp/utilities/settings.dart';

void main() async {
  // これを最初に実行しないとrunApp()の前にファイル等にアクセスできない。
  WidgetsFlutterBinding.ensureInitialized();
  final settings = SettingsProvider(Settings());
  await settings.initialise();
  
  runApp(
      ChangeNotifierProvider<SettingsProvider>(
        create: (_) => settings,
        child: MyApp()
      )
  );
}

class MyApp extends StatelessWidget {
  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider<ThermoModel>(
      create: (_) {
        final model = ThermoModel(
          apiClient: HttpApiClient(),
          url: Provider.of<SettingsProvider>(context).getApiURl(),
          deviceId: Provider.of<SettingsProvider>(context).getDeviceId(),
        )..fetchThermoData();

        Timer.periodic(Duration(seconds: 60), (_) => model.fetchThermoData());
        return model;
      },
      child: MaterialApp(
        title: 'GwThremo',
        theme: ThemeData(
          primarySwatch: Colors.pink,
        ),
        routes: <String, WidgetBuilder>{
          PagesNames.home: (_) => HomeScreen(),
          PagesNames.settings: (_) => SettingsScreen(),
        }
      ),
    );
  }
}
