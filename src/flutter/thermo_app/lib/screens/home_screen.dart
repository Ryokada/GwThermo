import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:thermoapp/pages_names.dart';
import 'package:thermoapp/provideres/thermo_provider.dart';
import 'package:thermoapp/widgets/graph_widget.dart';
import 'package:thermoapp/widgets/home_widget.dart';

class HomeScreen extends StatefulWidget {
  @override
  _HomeScreenState createState() => _HomeScreenState();
}

enum _HomeScreenTab {
  home,
  graph
}

class _HomeScreenState extends State<HomeScreen> {
  final _tab = ValueNotifier(_HomeScreenTab.home);

  @override
  void initState(){
    //アプリ起動時に一度だけ実行される

    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('暇'),
        actions: <Widget>[
          IconButton(
            icon: Icon(Icons.update),
            onPressed: () => Provider.of<ThermoModel>(context, listen: false).fetchThermoData(),
          )
        ],

      ),
      drawer: Drawer(
        child: ListView(
          children: <Widget>[
            DrawerHeader(
              child: Text(
                "サイドバー",
                style: TextStyle(
                  fontSize: 24,
                  color: Colors.white,
                ),
              ),
              decoration: BoxDecoration(
                color: Colors.pink,
              ),
            ),
            ListTile(
              title: Text("設定"),
              // TODO: 設定画面の実装
              onTap: () {
                Navigator.pop(context);
                Navigator.pushNamed(context, PagesNames.settings);
              },
              trailing: Icon(
                Icons.navigate_next
              ),
            )
          ],
        ),
      ),
      body: Selector<ThermoModel, bool>(
        selector: (context, model) => model.isLoading,
        builder: (context, isLoading, _) {
          if (isLoading){
            return Center(
              child: CircularProgressIndicator(),
            );
          }
          return ValueListenableBuilder<_HomeScreenTab>(
            valueListenable: _tab,
            builder: (context, tab, _) {
              switch(tab){
                case _HomeScreenTab.graph: {
                  return Graph();
                }
                case _HomeScreenTab.home:
                default: {
                  return Home();
                }
              }
            },
          );
        },
      ),
      bottomNavigationBar: ValueListenableBuilder<_HomeScreenTab>(
        valueListenable: _tab,
        builder: (context, tab, _) {
          return BottomNavigationBar(
            currentIndex: _HomeScreenTab.values.indexOf(tab),
            onTap: (int index) => _tab.value = _HomeScreenTab.values[index],
            items: [
              BottomNavigationBarItem(
                icon: Icon(Icons.list),
                title: Text("ホーム"),
              ),
              BottomNavigationBarItem(
                icon: Icon(Icons.show_chart),
                title: Text("グラフ"),
              ),
            ],
          );
        },
      ),
    );
  }

  @override
  void dispose() {
    _tab.dispose();
    super.dispose();
  }
}

