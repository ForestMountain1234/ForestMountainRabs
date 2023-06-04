class State:
    def __init__(self, init_value):
        self.__value = init_value
        self.__on_change_events = []

    #値を取得する。
    def get(self):
        return self.__value

    #値を設定し、変更時イベントを全て実行する。
    def set(self, new_value):
        self.__value = new_value
        #値のセット時に変更時に呼び出すよう登録した関数を実行
        for event in self.__on_change_events:
            event(new_value)

    #値の変更時に呼び出される関数を登録する。
    def databind(self, on_change_event):
        self.__on_change_events.append(on_change_event)

class Store:
    def __init__(self):
        self.panel = State('Main')
        self.text = State('')