import flet as ft
from state import StateProperty, bind_props, get_prop_value


class ReactiveText(ft.UserControl):
    def __init__(self, text: StateProperty[str], size: StateProperty[int] = 17):
        super().__init__()
        self.control = ft.Text('')
        self.text = text
        self.size = size

        self.set_props()
        bind_props([self.text, self.size], lambda: self.update())

    def set_props(self):
        self.control.value = get_prop_value(self.text)
        self.control.size  = get_prop_value(self.size)

    def update(self):
        self.set_props()
        self.control.update()

    def build(self):
        return self.control
