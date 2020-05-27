import 'dart:collection';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:thermoapp/models/thermo_telemetry_data.dart';
import 'package:thermoapp/repositories/api_client.dart';

class ThermoModel extends ChangeNotifier {
  final ApiClient apiClient;
  final String url;
  final String deviceId;

  bool _isLoading = false;
  bool get isLoading => _isLoading;

  List<ThermoTelemetryData> _thermoData = [];
  UnmodifiableListView<ThermoTelemetryData> get thermoData => UnmodifiableListView(_thermoData);

  ThermoModel({
    @required this.apiClient,
    @required this.url,
    @required this.deviceId,
    List<ThermoTelemetryData> thermoData,
  }) : _thermoData = thermoData ?? [];

  Future fetchThermoData() async {
    _isLoading = true;
    notifyListeners();

    try{
      final response = await apiClient.getAsync("$url/api/thermo/getdata/$deviceId", headers: { "top": "10" });


      List<ThermoTelemetryData> results = (json.decode(response.body) as List).map((e) => new ThermoTelemetryData.fromJson(e)).toList();
//        await Future.delayed(Duration(seconds: 5));
//        final results = [
//          ThermoTelemetryData(
//            "deviceId",
//            DateTime(2020, 5, 10, 10, 10),
//            [
//              <double>[25,25.25,26.25,27.5,27.25,26.75,25.5,24],
//              <double>[24,23.75,26,27.25,27.25,26.75,24.5,24],
//              <double>[22.5,23.25,23.5,25.25,28,24.25,23.75,23.5],
//              <double>[21.75,22.5,23,24,26,23.5,23,23],
//              <double>[22.25,22.75,22.75,23,22.75,23.5,23,22.75],
//              <double>[21.5,22.25,22,22.25,22.25,22.5,22.75,22.75],
//              <double>[21.25,22.25,22.5,22.25,22.5,22.25,22.25,22.25],
//              <double>[20.75,21.5,21.75,21.75,21.25,21.75,21,21]
//            ],
//            28
//          )
//        ];

      _thermoData = results;
    }
    catch(e) {
      // TODO: エラーハンドリング
    }
    _isLoading = false;
    notifyListeners();
  }
}