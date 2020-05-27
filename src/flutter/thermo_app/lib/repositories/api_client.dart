import 'dart:async';
import 'dart:io';

import 'package:http/http.dart' as http;
import 'package:retry/retry.dart';

/// APIリクエスト用のクライアント
abstract class ApiClient {
  final int maxRetryCount = 8;

  /// GETリクエストを送信します。
  Future<http.Response> getAsync(String url,
      {Map<String, String> queries,
        Map<String, String> headers,
        bool retry = true}) {
    if (headers == null) {
      headers = <String, String>{};
    }

    final options = RetryOptions(maxAttempts: retry ? maxRetryCount : 1);

    return options.retry(
          () => this.getAsyncCore(this._buildUrl(url, queries), headers),
      retryIf: (e) => e is SocketException || e is TimeoutException,
    );
  }

  /// GETリクエストのコア処理。
  Future<http.Response> getAsyncCore(String url, Map<String, String> headers);

  /// POSTリクエストを送信します。
  Future<http.Response> postAsync(String url,
      {String jsonBody,
        Map<String, String> queries,
        Map<String, String> headers,
        bool retry = true}) {
    if (headers == null) {
      headers = <String, String>{};
    }
    final options = RetryOptions(maxAttempts: retry ? maxRetryCount : 1);

//    headers['content-type'] = 'application/json';
    return this.postAsyncCore(this._buildUrl(url, queries), headers, jsonBody);
  }

  /// POSTリクエストのコア処理。
  Future<http.Response> postAsyncCore(
      String url, Map<String, String> headers, String jsonBody);

  /// クエリパラメータ付きのURLを構築します。
  String _buildUrl(String url, Map<String, String> queries) {
    if (queries == null) {
      return url;
    }
    var isFirst = true;
    var builder = StringBuffer(url);
    var delimiter = '?';
    queries.forEach((key, value) {
      builder.write(delimiter);
      builder.write(key);
      builder.write('=');
      builder.write(value);
      if (isFirst) {
        isFirst = false;
        delimiter = '&';
      }
    });
    return builder.toString();
  }
}
