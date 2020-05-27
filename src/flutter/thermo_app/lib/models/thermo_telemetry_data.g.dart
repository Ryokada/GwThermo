// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'thermo_telemetry_data.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ThermoTelemetryData _$ThermoTelemetryDataFromJson(Map<String, dynamic> json) {
  return ThermoTelemetryData(
    json['deviceId'] as String,
    json['timestamp'] == null
        ? null
        : DateTime.parse(json['timestamp'] as String),
    (json['data'] as List)
        ?.map((e) => (e as List)?.map((e) => (e as num)?.toDouble())?.toList())
        ?.toList(),
    (json['maxValue'] as num)?.toDouble(),
  );
}

Map<String, dynamic> _$ThermoTelemetryDataToJson(ThermoTelemetryData instance) {
  final val = <String, dynamic>{};

  void writeNotNull(String key, dynamic value) {
    if (value != null) {
      val[key] = value;
    }
  }

  writeNotNull('deviceId', instance.deviceId);
  writeNotNull('timestamp', instance.timestamp?.toIso8601String());
  writeNotNull('data', instance.data);
  writeNotNull('maxValue', instance.maxValue);
  return val;
}
