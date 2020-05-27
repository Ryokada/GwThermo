import "package:intl/intl.dart";
import 'package:intl/date_symbol_data_local.dart';

class DateTimeFormatter {
  static String formatToJST(DateTime dateTime){
    initializeDateFormatting("ja_JP");
    final formatter = DateFormat("yyyy/MM/dd HH:mm", "ja_JP");
    return formatter.format(dateTime);
  }
}