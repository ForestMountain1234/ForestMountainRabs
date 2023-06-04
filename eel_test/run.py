import eel


def main():
    port = 10001  # 好きなポートで
    eel.init("./web/build")  # 使用するhtml/js/cssのあるフォルダを指定
    eel.start("index.html", port=port)  # 最初の画面index.htmlを指定

# @eel.expose()を指定してJavaScriptから呼べるように関数を登録
@eel.expose
def test_eel():
    print("from react.")


main()