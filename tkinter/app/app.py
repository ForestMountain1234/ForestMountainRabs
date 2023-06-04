import customtkinter
from main_frame import MainFrame
from sub_frame import SubFrame
from store import Store

FONT_TYPE = "meiryo"

class App(customtkinter.CTk):
    def __init__(self):
        super().__init__()

        self.fonts = (FONT_TYPE, 15)
        self.geometry("1080x800")
        self.title("GUI Test")
    
        self.store = Store()
        self.store.panel.databind(self.panel_bind)

        self.setup_form()
    
    def setup_form(self):
        customtkinter.set_appearance_mode("dark")
        customtkinter.set_default_color_theme("blue")
        self.main = MainFrame(self, self.store).get()
        self.sub = SubFrame(self, self.store).get()

    def panel_bind(self, val: str):
        if val == 'Main':
            self.main.tkraise()
        elif val == 'Sub':
            self.sub.tkraise()

if __name__ == "__main__":
    # アプリケーション実行
    app = App()
    app.mainloop()
    