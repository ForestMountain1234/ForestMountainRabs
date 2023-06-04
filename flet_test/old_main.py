import flet as ft

class Counter(ft.UserControl):
    def __init__(self):
        super().__init__(self)
        #追加したプロパティ
        self.counter = 0 
        self.on_countup = lambda: None
        self.text = ft.Text(str(self.counter))

    def count_up(self):
        self.counter += 1
        self.text.value = str(self.counter)
        self.update()

    def on_click_handler(self, e):
        self.count_up()
        self.on_countup() 

    def build(self):
        return ft.Row([self.text, ft.ElevatedButton("Add", on_click=self.on_click_handler)])

def main(page: ft.Page):
    page.title = "Flet example"
    counter1 = Counter()
    counter2 = Counter()
    counter1.on_countup = lambda: counter2.count_up()
    counter2.on_countup = lambda: counter1.count_up()
    page.add(counter1, counter2)

ft.app(target=main)