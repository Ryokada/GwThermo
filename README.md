# ラズパイで非接触温度センサーで体温を測って、可視化するアプリ

ラズパイに繋がった非接触温度センサーを定期的クラウドに送信し、モバイルアプリで可視化する仕組みです。

# 構成

センサ− -> ラズパイ -> Azure IoTHub -> Azure Functions -> Azure CosmosDB <- Azue AppServices <- モバイルアプリ
