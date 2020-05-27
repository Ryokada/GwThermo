import 'dart:convert';
import 'dart:async';

import 'package:http/http.dart' as http;
import 'api_client.dart';

/// 実際にHTTPリクエストを送信する [HttpApiClient] の実装です。
class HttpApiClient extends ApiClient {
  static final HttpApiClient _singleton = HttpApiClient._internal();

  factory HttpApiClient() {
    return _singleton;
  }

  HttpApiClient._internal();

  final int timeoutSeconds = 10;

  @override
  Future<http.Response> getAsyncCore(String url, Map<String, String> headers) =>
      http
          .get(url, headers: headers)
          .timeout(Duration(seconds: timeoutSeconds));

  @override
  Future<http.Response> postAsyncCore(
      String url, Map<String, String> headers, String jsonBody) =>
      http
          .post(
        url,
        headers: headers,
        body: jsonBody,
//              encoding: Encoding.getByName('utf-8'))
      )
          .timeout(Duration(seconds: timeoutSeconds));
}
