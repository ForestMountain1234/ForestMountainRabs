import customtkinter
from store import Store
FONT_TYPE = "meiryo"

class MainFrame:
    def __init__(self, master: customtkinter.CTk, store: Store):
        self.fonts = (FONT_TYPE, 12)

        self.frame = customtkinter.CTkFrame(master, width=1080, height=800)
        self.frame.grid(row=0, column=0, sticky="nsew", pady=20)
        self.store = store

        # 各種ウィジェットの作成
        label1_frame = customtkinter.CTkLabel(self.frame, text="メインウィンドウ", font=self.fonts)
        entry1_frame = customtkinter.CTkEntry(self.frame, font=self.fonts)
        button_change = customtkinter.CTkButton(self.frame, text="アプリウィンドウに移動", font=self.fonts, command=self.button_onclick)
        # 各種ウィジェットの設置
        label1_frame.pack()
        entry1_frame.pack()
        button_change.pack()

    def get(self):
        return self.frame

    def button_onclick(self):
        self.store.panel.set('Sub')