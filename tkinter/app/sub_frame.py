import customtkinter
from store import Store

FONT_TYPE = "meiryo"

class SubFrame:
    def __init__(self, master: customtkinter.CTk, store: Store):
        self.fonts = (FONT_TYPE, 12)
        self.store = store

        # アプリフレームの作成と設置
        self.frame_app = customtkinter.CTkFrame(master)
        self.frame_app.grid(row=0, column=0, sticky="nsew", pady=20)

        # 各種ウィジェットの作成
        label1_frame_app = customtkinter.CTkLabel(self.frame_app, text="アプリウィンドウ", font=self.fonts)
        entry1_frame_app = customtkinter.CTkEntry(self.frame_app, font=self.fonts)
        button_change_frame_app = customtkinter.CTkButton(self.frame_app, text="メインウィンドウに移動", font=self.fonts, command=self.button_onclick)

        # 各種ウィジェットの設置
        label1_frame_app.pack()
        entry1_frame_app.pack()
        button_change_frame_app.pack()

    def get(self):
        return self.frame_app

    def button_onclick(self):
        self.store.panel.set('Main')