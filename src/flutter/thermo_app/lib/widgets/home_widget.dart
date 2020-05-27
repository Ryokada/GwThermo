import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:thermoapp/models/thermo_telemetry_data.dart';
import 'package:thermoapp/provideres/thermo_provider.dart';
import 'package:thermoapp/utilities/datetime_formatter.dart';
import 'package:thermoapp/widgets/space_box.dart';
import 'package:thermoapp/widgets/thermo_matrix.dart';

class Home extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Container(
      margin: EdgeInsets.symmetric(vertical: 20, horizontal: 20),
      child: Selector<ThermoModel, List<ThermoTelemetryData>>(
        // TODO: データ0件の考慮
        selector: (_, model) => model.thermoData,
        builder: (context, thermoData, _) {
          return Column(
            children: <Widget>[
              Row(
                mainAxisAlignment: MainAxisAlignment.start,
                children: <Widget>[
                  Text(
                    "最近の体温",
                    style: TextStyle(
                        color: Colors.black,
                        fontSize: 18),
                  ),
                ],
              ),
              SpaceBox(),
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  Text(
                    "${thermoData.first.maxValue.toStringAsFixed(1)}℃",
                    style: TextStyle(
                        color: Colors.black,
                        fontSize: 60,
                        fontWeight: FontWeight.bold),
                  ),
                ],
              ),
              Row(
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  Text(
                    "${DateTimeFormatter.formatToJST(thermoData.first.timestamp)}",
                    style: TextStyle(
                        color: Colors.black,
                        fontSize: 18),
                  ),
                ],
              ),
              SpaceBox(),
              ThermoMatrix()
            ],
          );
        },
      ),
    );
  }
}