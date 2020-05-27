import 'package:json_annotation/json_annotation.dart';

part 'thermo_telemetry_data.g.dart';

@JsonSerializable(includeIfNull: false)
class ThermoTelemetryData {
  @JsonKey(name: "deviceId")
  final String deviceId;
  @JsonKey(name: "timestamp")
  final DateTime timestamp;
  @JsonKey(name: "data")
  final List<List<double>> data;
  @JsonKey(name: "maxValue")
  final double maxValue;

  ThermoTelemetryData(this.deviceId, this.timestamp, this.data, this.maxValue);

  factory ThermoTelemetryData.fromJson(Map<String, dynamic> json) =>
      _$ThermoTelemetryDataFromJson(json);
}