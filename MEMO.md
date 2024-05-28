開発メモ
========

2024-04-24
----------

### 設定ファイル操作

* 設計指針
    * 祝日情報(`syukujitsu.csv`)の読み込みは必須のため、CSVパーザが必要。
    * その他に必要な情報も複雑な構造ではないため、全てCSV型式とする。
* 処理概要
    * 共通
        * アクセス先フォルダの決定
            * `AppData\Local\SimpleCalendar\`
            * 実アプリ用とユニットテスト用で実装を切り換える? → 別サービス化
        * 初回起動時の設定ファイルの取り扱い
            * 対象ファイルがユーザーディレクトリに無ければ、アプリ内に保持したファイルをコピーする。
        * 読み出すファイルはユーザーディレクトリ側のファイルとする。
            * 読み出し時にエラーになった場合、アプリ内に保持したファイルにフォールバックする。
                * ステータス情報を設定画面で見れるようにした方がよい?
        * 設定ファイルの種類
            * `syukujitsu.csv`: [「国民の祝日」について (内閣府)](https://www8.cao.go.jp/chosei/shukujitsu/gaiyou.html) から取得したデータ。
                * 以下のURLにアクセスして、アプリ内で情報をアップデートする想定。
                    * https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv
                        * URLはおそらく変わらないはず……。
                    * 自動的に取得して非同期にアップデートしようとすると、非同期処理をいろいろ考えないといけないので、とりあえず明示的に取得するボタンか何かを用意する。
            * `specialdays.csv`: ユーザーが手書きで特別日を登録。(実装をシンプルにするため、年月日の指定を必須とする)
            * `daysofweek.csv`: 各曜日の種別(平日/休日など)を登録。土日が休日でない場合に対応。
    * 設定読み込み
        * ファイルのタイムスタンプをチェックして自動的更新することを考えたが、上と同様非同期処理の問題があるので、明示的に読み込むボタンか何かを用意する。
        * サービスではなくモデルで実装する?
* 変更点
    * 曜日毎のスタイル設定を外部化するのに合わせて、DayTypeを見直し。

2024-04-25
----------

* [C# - GetEncodingなどでshift_jisを指定すると例外が発生する](https://www.curict.com/item/72/72d5fb2.html)
    * なん……だと……!?

2024-04-26
----------

* ModelとViewModelとServiceの設計にあたってのポリシーがよくわからない。識者求む(´･ω･`)

2024-04-27
----------

* スタイルを動的に変更しようとすると実行時例外。
    * StaticResouceで参照すると、スタイルがsealされて以後変更不可になる。 ~~DynamicResourceを使えばよいらしい。(未確認)~~ ⇒ ダメでした。そもそもスタイルが「使用」されている時点で内部構造を変更するのは不可の模様。
        * TriggerのSetterでDayLabelStyleSettingsのプロパティにバインドする形でお茶を濁す。
            * あんまり細かくスタイル指定をしたいという需要もないだろうし……。
* ResourceDictionaryを外部ファイル化すると、実行時は問題ないのに、VS2022のXAMLデザイナーでエラー(「XDG0003 リソース 'ResourceDictionaryファイル名' を検索できません。」)が発生する。
    * MainWindow.xamlのみで発生する。CalendarMonthView.xamlでは発生しない。
        * エラーメッセージをよく見ると、CalendarMonthView.xamlのSourceでは絶対パスでファイルを指定している(先頭に「/」を付けている)のに、メッセージではパス名の先頭に「/」が付いていない。
            * Sourceを相対パス指定に変更するとエラー解消。おそらくUserControlでResourceDictionaryを外部化すると同様の問題が発生すると思われる。
* 既存の実装だとマウスオーバー関するトリガーの条件設定がいろいろ複雑化するので、DayLabelにIsDayTypeEmptyを足してMultiTriggerにすることでシンプル化した。
    * 合わせて、DayLabel.DayTypeとDayOfWeekLabel.DayOfWeekをstringからenumに変更。(これでインテリセンスが効くはず)
        * DayOfWeekLabel.DayOfWeek設定時、自動的にContentを設定。(国際化を入れるのが面倒なので、直接日本語文字列を設定)
* マウスホイールの回転に対応。InputBindingがマウスホイールの回転に未対応のため、コードビハインドで直接書く。

2024-04-28
----------

* 通知領域(タスクトレイ)への登録。
    * WPFは未対応(!!)
    * workaroundとして、Windows Formsを使用している例が大半だが、いくつか問題があるらしい。
        * 参考: [タスクトレイ常駐アプリの実装 Tips＆Tricks（その２）](https://hnx8.hatenablog.com/entry/20131102/1383415896)
    * WPFを使いながらWindows Formsへも依存するのは何か負けた気がするので、Win32APIを直接叩く方法を試行。
        * [C#/Win32 P/Invoke Source Generator](https://github.com/microsoft/CsWin32)
            * [\[C#\] CsWin32でWin32APIのプラットフォーム呼び出し(P/Invoke)コードを自動生成](https://qiita.com/radian-jp/items/a4509f9a44101fb2f30e)
        * [C# Win32API完全入門](https://qiita.com/nekotadon/items/f376d17de85dfb84fbd5)
    * [通知と通知領域](https://learn.microsoft.com/ja-jp/windows/win32/shell/notification-area)
        * [Shell_NotifyIconW 関数 (shellapi.h)](https://learn.microsoft.com/ja-jp/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw)
        * [NOTIFYICONDATAW](https://learn.microsoft.com/ja-jp/windows/win32/api/shellapi/ns-shellapi-notifyicondataw)
        * [NotificationIcon サンプル](https://learn.microsoft.com/ja-jp/windows/win32/shell/samples-notificationicon)
            * [NotificationIcon.cpp](https://github.com/microsoft/Windows-classic-samples/blob/main/Samples/Win7Samples/winui/shell/appshellintegration/NotificationIcon/NotificationIcon.cpp)

2024-04-29
----------

* WPFで独自イベントハンドラ: いくつかバリエーションがあるけどどれが適切なのかわからない……。
    * [How to handle WndProc messages in WPF?](https://stackoverflow.com/questions/624367/how-to-handle-wndproc-messages-in-wpf)
    * [PresentationSource.FromVisual(this) returns null value in WPF](https://stackoverflow.com/questions/11204251/presentationsource-fromvisualthis-returns-null-value-in-wpf)
    * [WPFアプリケーションでウィンドウプロシージャをフックする](https://qiita.com/tricogimmick/items/86141bc33c0e06e9d2e9)
* プロジェクトGUIDが欲しい。
    * `*.sln`ファイルが書いてあるのがGUID?
        * `Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "SimpleCalendar.WPF", "SimpleCalendar.WPF\SimpleCalendar.WPF.csproj", "{060F633E-8DB1-46D0-A1A4-9DCF5F9DB46C}"`
            * 1つ目(`Project(...)`) ⇒ Visual Studioで規定されたプロジェクトタイプGUID
                * [What are the project GUIDs in a Visual Studio solution file used for?](https://stackoverflow.com/questions/2327202/what-are-the-project-guids-in-a-visual-studio-solution-file-used-for)
                * [Visual Studio Project Type Guids](https://github.com/JamesW75/visual-studio-project-type-guid)
                    * オリジナル: [List of Visual Studio Project Type GUIDs](https://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs)
            * 2つ目(末尾にあるもの) ⇒ プロジェクトGUID
                * [How do I programmatically get the GUID of an application in C# with .NET?](https://stackoverflow.com/questions/502303/how-do-i-programmatically-get-the-guid-of-an-application-in-c-sharp-with-net)
    * [How to get project GUID in runtime of .netcore?](https://stackoverflow.com/questions/61071849/how-to-get-project-guid-in-runtime-of-netcore)
        * .NET Coreでは、`*.sln`や`*.csproj`に記述されたプロジェクトGUIDはアセンブリには埋め込まれないらしい?
            * 単にプログラム実行期間中だけのGUIDが欲しいだけなら、`Guid.NewGuid()`の結果をどこかに保存すればよい。

2024-05-03
----------

* 通知領域(タスクトレイ)への登録その2。
    * 通知領域への登録は、以下の2段階で行う必要がある。(詳細は前述の [NotificationIcon.cpp](https://github.com/microsoft/Windows-classic-samples/blob/main/Samples/Win7Samples/winui/shell/appshellintegration/NotificationIcon/NotificationIcon.cpp) を参照のこと。2段階必要なことに気付かず、`NIM_ADD | NIM_SETVERSION` で設定しようとしていた……)
        1. `Shell_NotifyIcon(NIM_ADD, ...)` で諸々の情報を登録する。
        2. `Shell_NotifyIcon(NIM_SETVERSION, ...)` で対応バージョンを昇格する。

2024-05-07
----------

* 諸々のバグ修正。
* 「発行」したファイル群をzipにアーカイブする設定を追加。
* Excelが設定ファイルを開いていても読み込めるよう対応。(参考: [How do I open an already opened file with a .net StreamReader?](https://stackoverflow.com/a/898017))

2024-05-08
----------

* `.editorconfig` を [dotnet/runtime](https://github.com/dotnet/runtime) からほぼそのまま取り込み。(ただし `file_header_template` は同プロジェクト固有なので削除)
    * 「コードクリーンアップの実行」を実施 + メッセージでの指摘に対応。
* 通知アイコンに設定するGUIDを固定。(毎回生成すると、タスクバーに表示するアイコンの選択画面で同じアプリが増殖する)

2024-05-13
----------

* 別ウィンドウを生成するとき、Ownerを設定するとウィンドウ間に親子関係ができる。(生成されたウィンドウが常に生成元のウィンドウの前に来る、など) 単にウィンドウを開くだけにして、生成元のウィンドウを関連性を持たせたくないならOwnerは設定しない。
* ヘルプについていろいろ検討した結果、XAMLファイルに直接書き込む方針に決定。

2024-05-15
----------

* https://www8.cao.go.jp/chosei/shukujitsu/syukujitsu.csv のEtag、中身変わってないのに値が変わることがあるのは何でなん……。あと「If-Modified-Since」も見てないっぽい。

2024-05-21
----------

* WPFのデータバインディングでは、プロパティの更新をUIスレッドで行う必要はない?
    * ない。参考: [バインドしてるObservableCollectionを非UIスレッドから操作する](https://sourcechord.hatenablog.com/entry/2014/02/02/153725)

2024-05-22
----------

* Windowsアプリケーションの改行コードをLFに統一するのは止めた方がよさそう。ちょいちょいgitが警告を出したり無駄に差分が出たりするので。

2024-05-27
----------

* XAML Styler設定
    * Keep first attribute on same line: False → True
        * 最初の属性をタグ名と同じ行にする & インデントを最初の属性の位置に合わせる
    * Attribute tolerance: 2 (変更しない)
        * 属性数が指定値以上だと複数行に分ける (「2」なら2つ以上属性があれば複数行に分ける)

2024-05-28
----------

* WinUI3、SizeToContent相当の実装がかなり難しい……。

以上
