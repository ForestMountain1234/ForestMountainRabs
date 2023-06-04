from typing import TypeVar, Generic, Union, Callable

T = TypeVar('T')

#状態管理クラス。bind()で状態変更時に呼び出したい処理を登録できる。
class State(Generic[T]):
    def __init__(self, value: T):
        self._value = value
        self._observers: list[Callable] = []

    def get(self):
        return self._value #値の参照はここから

    def set(self, new_value: T):
        if self._value != new_value:
            self._value = new_value #新しい値をセット
            for observer in self._observers: observer() #変更時に各observerに通知する

    def bind(self, observer):
        self._observers.append(observer)# 変更時に呼び出す為のリストに登録

    def unbind(self, observer):
        self._observers.remove(observer)

# 依存しているStateの変更に応じて値が変わるクラス。
class ReactiveState(Generic[T]):
    def __init__(self, formula: Callable[[], T], reliance_states: list[State]):
        self.__value = State(formula())
        self.__formula = formula
        self._observers: list[Callable] = []

        for state in reliance_states:
            state.bind(lambda : self.update())

    def get(self):
        return self.__value.get()

    def update(self):
        old_value = self.__value.get()
        self.__value.set(self.__formula())

        if old_value != self.__value.get():
            for observer in self._observers: observer() #変更時に各observerに通知する

    def bind(self, observer):
        self._observers.append(observer)# 変更時に呼び出す為のリストに登録

    def unbind(self, observer):
        self._observers.remove(observer)


StateProperty = Union[T, State[T], ReactiveState[T]]

def bind_props(props: list[StateProperty], bind_func: Callable[[], None]):
    for prop in props:
        if isinstance(prop, State) or isinstance(prop, ReactiveState):
            prop.bind(lambda : bind_func())


def get_prop_value(prop: StateProperty):
    if isinstance(prop, State):
        return prop.get()
    elif isinstance(prop, ReactiveState):
        return prop.get()
    else:
        return prop


