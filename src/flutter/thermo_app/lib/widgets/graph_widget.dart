import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:thermoapp/models/thermo_telemetry_data.dart';
import 'package:thermoapp/pages_names.dart';
import 'package:thermoapp/provideres/thermo_provider.dart';
import 'package:thermoapp/widgets/graph_widget.dart';
import 'package:thermoapp/widgets/home_widget.dart';
import 'package:charts_flutter/flutter.dart' as charts;

class Graph extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Selector<ThermoModel, List<ThermoTelemetryData>>(
      selector: (context, model) => model.thermoData,
      builder: (context, thermoData, _) {
        return Column(
          children: <Widget>[
            Text("グラフ"),
            // TODO: データ0件の考慮
            Expanded(child: Container(height: 500, child: Chart(thermoData)))
          ]
        );
      },
    );
  }
}

class Chart extends StatelessWidget {
  final List<ThermoTelemetryData> seriesList;
  final bool animate = false;

  Chart(this.seriesList);

  @override
  Widget build(BuildContext context) {
    return charts.TimeSeriesChart(
      [
        charts.Series<ThermoTelemetryData, DateTime>(
          id: "thermo_data",
          colorFn: (_, __) => charts.MaterialPalette.blue.shadeDefault,
          domainFn: (ThermoTelemetryData data, _) => data.timestamp,
          measureFn: (ThermoTelemetryData data, _) => data.maxValue,
          data: seriesList
        ),
      ],
      animate: animate,
      dateTimeFactory: const charts.LocalDateTimeFactory(),
    );
  }
}