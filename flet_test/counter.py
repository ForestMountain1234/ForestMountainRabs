import flet as ft

class InfoAria(ft.UserControl):
    def __init__(self):
        super().__init__(self)
        self.text_len_label = ft.Text('文字数:')
        self.text_val_label = ft.Text('テキストボックスに「」と入力されています。')

    def on_change_handler(self, e):
        value = e.control.value
        self.text_len_label.value = f'文字数:{len(value)}'
        self.text_val_label.value = f'テキストボックスに「{value}」と入力されています。'
        self.text_len_label.update()
        self.text_val_label.update()

    def build(self):
        return ft.Column([self.text_len_label, self.text_val_label])

def main(page: ft.Page):
    page.title = "Flet example"
    info_aria = InfoAria()

    text_box = ft.TextField(on_change=info_aria.on_change_handler)    
    page.add(ft.Column([text_box, info_aria]))

ft.app(target=main)