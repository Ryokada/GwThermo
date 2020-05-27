import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:thermoapp/models/thermo_telemetry_data.dart';
import 'package:thermoapp/provideres/thermo_provider.dart';

class ThermoMatrix extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Card(
      child: Selector<ThermoModel, ThermoTelemetryData>(
        selector: (_, model) => model.thermoData.first,
        builder: (context, thermoData, _){
          return Container(
            height: 300,
            width: 300,
            child: CustomPaint(painter: _MyPainter(thermoData.data)),
          );
        }
      ),
    );
  }
}

class _MyPainter extends CustomPainter {
  final List<List<double>> _data;

  _MyPainter(this._data);

  @override
  bool shouldRepaint(_MyPainter oldDelegate) {
    return true;
  }

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint();

    paint.color = Colors.white;
    var rect = Rect.fromLTWH(0, 0, size.width, size.height);
    canvas.drawRect(rect, paint);

    // 四角（塗りつぶし）
    for (var y = 0; y < _data.length; y++){
      for (var x = 0; x < _data[y].length; x++){
        paint.color = _getColor(_data[y][x]);
        final path = Path();
        path.moveTo((30 * (x + 1)).toDouble(), (30 * (y + 1)).toDouble());
        path.lineTo((30 * (x + 2)).toDouble(), (30 * (y + 1)).toDouble());
        path.lineTo((30 * (x + 2)).toDouble(), (30 * (y + 2)).toDouble());
        path.lineTo((30 * (x + 1)).toDouble(), (30 * (y + 2)).toDouble());
        path.close();
        canvas.drawPath(path, paint);
      }
    }
  }

  Color _getColor(double temp){
    final normal = 28.0;
    if (temp < normal - 4){
      return Colors.grey;
    }
    if (temp < normal - 3){
      return Colors.blue;
    }
    if (temp < normal - 2){
      return Colors.lightBlue;
    }
    if (temp < normal - 1){
      return Colors.lightGreen;
    }
    if (temp < normal){
      return Colors.green;
    }
    if (temp < normal + 1){
      return Colors.amber;
    }
    if (temp < normal + 2){
      return Colors.deepOrange;
    }
    if (temp < normal + 3){
      return Colors.red;
    }

    return Colors.black;
  }
}
