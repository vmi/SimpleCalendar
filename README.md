SimpleCalendar - シンプルなカレンダーアプリケーション
=====================================================

WPFアプリケーション作成の習作として作成したもの。

* 画面全体を掴んでドラッグで移動可能。
* 右クリックメニューでヘルプ表示および終了の選択。

⇒ [開発メモ](MEMO.md)

学習した内容
------------

* WPF
    * XAMLで画面を作る
        * レイアウト (StackPanel/Grid)
        * データバインディング
            * ラベルとViewModelとのデータバインディング
                * ViewModelの複数のプロパティを使用するマルチバインディング
        * スタイルで画面デザイン(色など)を設定する
        * 実行画面とXAMLデザイナー画面の両方で使用可能なViewModelの割り当て方法
        * キーボードショートカット
        * 右クリックメニュー
    * 既存コントロールの拡張
    * ユーザーコントロール(UserControl)の作成
    * ResouceDictionaryの外部ファイル化
* MVVM
    * ViewとViewModelの連携 (ViewModelとModelの連携はまだ)
        * 依存プロパティの作り方
        * 添付プロパティの作り方
        * コマンドの実行
* MVVM Toolkit (Community Toolkits for .NET) の使い方
* 依存性注入 (Dependency Injection)
* xUnit (さわりだけ)
* Windowsプラットフォーム操作
    * CsWin32
    * 通知領域 (タスクトレイ)
    * WPFへの独自イベントハンドラの追加

TODO
----

* ✔ ~~曜日の部分も色を変える~~
* ✔ ~~マウスカーソル位置の日付の色変え~~
* ✔ ~~祝祭日対応~~
    * ✔ ~~サイトからCSVを取ってきて設定情報ディレクトリに保存する~~
* ✔ ~~設定画面を作る~~
* ユーザーのディレクトリで設定情報の保存と取得を行う
    * ✔ ~~祝祭日データ~~
        * インターネットから最新版の取得と保存機能
    * ✔ ~~ユーザーカスタマイズデータ~~
        * ✔ ~~年月日指定情報~~
            * ~~ウィークデイの休日設定 (曜日指定)~~ ⇒ ✔ 曜日毎に文字色を指定可能に
            * ✔ ~~年月日固定の日~~
            * ⚠ 以下の機能はデータ構造や処理がえらく複雑化することが想定された(=「シンプル」から外れる)ので対応しないことに決定
                * ✘ ~~週毎の予定日 (曜日、開始日orNULL、終了日orNULL)~~
                * ✘ ~~月毎の予定日 (日or第x週、開始日orNULL、終了日orNULL)~~
                * ✘ ~~年毎の予定日 (月/日、開始日orNULL、終了日orNULL)~~
        * ✔ ~~表示色~~
            * ✔ ~~曜日色~~
                * ✘ ~~平日~~
                * ✔ ~~日, 月, 火, 水, 木, 金, 土~~
            * ✔ ~~祝祭日~~
            * ✔ ~~ユーザー定義 (3種類くらい?)~~
        * フォント名
        * フォントサイズ
        * ✔ ~~設定変更時の再読み込み機能~~
* ✔ ~~配布物生成対応 (zipを生成)~~
* ✔ ~~メニューにアイコンを付ける~~
* ✔ ~~ヘルプを書く~~

参考資料および現時点での知見
============================

C#言語
------

* [C#で文字列の中に変数を埋め込む](https://qiita.com/YoshijiGates/items/3e88a8aee51001014ed7)
* `null` と `default` の使い分けがわからない。(資料求む)
* [アクセス修飾子 (C# プログラミング ガイド)](https://learn.microsoft.com/ja-jp/dotnet/csharp/programming-guide/classes-and-structs/access-modifiers)
* [Encodingクラスで扱えるエンコーディング名は？［C#／VB］](https://atmarkit.itmedia.co.jp/ait/articles/0304/11/news004.html)
    * [shift_jis](https://www.iana.org/assignments/charset-reg/shift_jis)と[windows-31J(CP932, MS932とも)](https://www.iana.org/assignments/charset-reg/windows-31J)ではUnicodeマッピングが異なる([shift_jisの場合](https://unicode.org/Public/MAPPINGS/OBSOLETE/EASTASIA/JIS/SHIFTJIS.TXT)、[windows-31Jの場合](https://unicode.org/Public/MAPPINGS/VENDORS/MICSFT/WINDOWS/CP932.TXT))ので、Java等の他言語では同一視すると不具合の要因になるのだが、C#の世界では同じものになっている模様。(.NET CoreアプリをWindows以外の環境に持っていくと不具合が出るかもしれない?(※要検証))
    * [C# - GetEncodingなどでshift_jisを指定すると例外が発生する](https://www.curict.com/item/72/72d5fb2.html)
        * 罠に注意。(事前に `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);` が必要)
* [StreamReader クラス](https://learn.microsoft.com/ja-jp/dotnet/api/system.io.streamreader?view=net-7.0) (.NET8の日本語版ドキュメントがないので.NET7版)
    * [StreamReader(Stream, Encoding, Boolean)](https://learn.microsoft.com/ja-jp/dotnet/api/system.io.streamreader.-ctor?view=net-7.0#system-io-streamreader-ctor(system-io-stream-system-text-encoding-system-boolean))
        * Encodingに`shift_jis`(もしくはコードページ932)を指定しつつ、第3引数をtrueにすると、BOM付きUTF-8を自動判定してくれる。

Windowsプラットフォーム操作
---------------------------

* [C#/Win32 P/Invoke Source Generator](https://github.com/microsoft/CsWin32)
* [通知と通知領域](https://learn.microsoft.com/ja-jp/windows/win32/shell/notification-area)
    * [Shell_NotifyIconW 関数 (shellapi.h)](https://learn.microsoft.com/ja-jp/windows/win32/api/shellapi/nf-shellapi-shell_notifyiconw)
    * [NOTIFYICONDATAW](https://learn.microsoft.com/ja-jp/windows/win32/api/shellapi/ns-shellapi-notifyicondataw)
    * [NotificationIcon サンプル](https://learn.microsoft.com/ja-jp/windows/win32/shell/samples-notificationicon)
        * [NotificationIcon.cpp](https://github.com/microsoft/Windows-classic-samples/blob/main/Samples/Win7Samples/winui/shell/appshellintegration/NotificationIcon/NotificationIcon.cpp)
            * 通知アイコン登録時、`Shell_NotifyIcon(NIM_ADD, &nid);` の実行後に `nid.uVersion = NOTIFYICON_VERSION_4;` を設定してから `Shell_NotifyIcon(NIM_SETVERSION, &nid);` を実行していることに注意。

WPF/XAML
--------

* [WPF4.5入門](https://blog.okazuki.jp/entry/20130102/1357124042)
    * ブックマークだけしてまだ読んでない(^^;
* [連載：WPF入門](https://atmarkit.itmedia.co.jp/ait/series/2794/)
    * [第4回 WPFの「リソース、スタイル、テンプレート」を習得しよう](https://atmarkit.itmedia.co.jp/ait/articles/1009/07/news096.html)
        * [VS2022] UserControlでResouceDictionaryを外部ファイル化した場合、UserControl定義側のSouce指定で絶対パスを指定すると、UserControl使用側のXAMLデザイナーで「XDG0003 リソース 'ResourceDictionaryファイル名' を検索できません。」というエラーが出て表示されない。実行時は正常で、VSのデザイン画面のみエラーとなる。Source指定を相対パスで行えばエラーにならない。
* [\[WPF\]*とAutoの違いをきちんと把握しておらずミスった話](https://qiita.com/nori0__/items/dbdc4aa4bec0f71857ca)
* [Set multibinding for a xaml element in code behind](https://stackoverflow.com/questions/5559397/set-multibinding-for-a-xaml-element-in-code-behind)
    * コードビハインドでマルチバインディングを実装。条件が複雑だと、コードとXAMLにとっちらかるよりコードの集約した方がよさそう?
* [XAML サービス](https://learn.microsoft.com/ja-jp/dotnet/desktop/xaml-services/)
    * 何が書いてあるのかよくわかっていないが、そのうち必要になるかもということでメモ。
* [XAMLデザイナ専用ViewModelコンストラクタの作り方](https://qiita.com/soi/items/17a78140cb032a4fed8c)
    * DIと組み合わせる場合は以下の「制御の反転 (MVVMツールキット)」の項も参照のこと。
* [WPFプロジェクトでリソースを正しく参照する](https://qiita.com/satodayo/items/b28cac887a6c34709682)
* [WPF アプリケーションのリソース ファイル、コンテンツ ファイル、およびデータ ファイル](https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/app-development/wpf-application-resource-content-and-data-files?view=netframeworkdesktop-4.8)
    * 資料は .NET Framework 4.x 向けだが、.NET 8 でも使用可能。
    * xUnit等では、packスキーム(pack URI)を使用可能にするための前処理が必要。
        * PackUriHelperクラスの初期化: `var s = System.IO.Packaging.PackUriHelper.UriSchemePack` (参考: [UriFormatException : Invalid URI: Invalid port specified](https://stackoverflow.com/questions/6005398/uriformatexception-invalid-uri-invalid-port-specified))
        * **ただし、本資料執筆時点では、 .NET 8 で xUnit を用いたテストコード内ではコンテンツファイルのストリームの取得に失敗する。** (`Application.GetContentStream(URI)` で `null` が返る)
            * WPF内部の処理を追い掛けてみたところ、コンテンツファイルなのにリソースファイルと認識されている模様。
            * [`ContentFilePart part = GetResourceOrContentPart(uriContent) as ContentFilePart;`](https://github.com/dotnet/wpf/blob/82260e0985c43d0a5304863f0b9c2d5fe70a3ce9/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Application.cs#L652) で、`GetResourceOrContentPart()` が何故か `ResourcePart` を返す。(それ以上は追いかけていない)
            * WPFのソースがGitHubで公開されているので、デバッガで追い掛けるときにフレームワークの内部まで見えるようにはできないだろうか? (今のところ手段が見付からず。上記はリフレクションで`private`なメソッドを直接呼び出して検証)
* 添付ビヘイビア: WPFのコンポーネントを機能拡張する際、直接継承するのではなく、添付ビヘイビアを実装して既存コンポーネントに添付(attach)する方式で実現することができる。
    * [View を支える添付プロパティ](https://qiita.com/kawasawa/items/36c18fdb512cc1bcbd54)
    * [添付ビヘイビアを作る](https://sourcechord.hatenablog.com/entry/2014/03/15/171857)
* [WPF Tips集（～するには）](https://qiita.com/Kosen-amai/items/ba13012a4076054df255)
* [コントロールのスタイルを作成する方法 (WPF .NET)](https://learn.microsoft.com/ja-jp/dotnet/desktop/wpf/controls/how-to-create-apply-style?view=netdesktop-8.0)
* [\[C#/WPF\] コンバーターの書き方](https://qiita.com/tera1707/items/47d1f1766cbe798b0c13)
* WPFデザインモードの判定: [DesignerProperties.GetIsInDesignMode(DependencyObject) Method](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.designerproperties.getisindesignmode?view=windowsdesktop-8.0)

MVVM
----

* [\[C#\]WPFでのMVVMについてサンプルアプリからまとめ](https://zenn.dev/naminodarie/articles/c9617df5ca879f)
    * アウトラインを把握するのにかなり参考になった。
* [MVVM Toolkit の概要](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/)
* [CommunityToolkit.Mvvm V8 入門](https://qiita.com/kk-river/items/d974b02f6c4010433a9e)
* [NET用 MVVM Toolkit v8でMVVMコードを短く](https://qiita.com/hqf00342/items/d12bb669d1ac6fed6ab6)
* [MVVMとは何か](https://days-of-programming.blogspot.com/2017/11/mvvm.html)
    * ModelとViewModelをどういう方針で設計したらよいかわからない状態で引っ掛かったページ。これが「正しい」かどうかはまだ判断が付かない。

C# の依存性注入 (DI/Dependency Injection)
-----------------------------------------

* [依存関係の挿入のガイドライン](https://learn.microsoft.com/ja-jp/dotnet/core/extensions/dependency-injection-guidelines)
* [制御の反転 (MVVMツールキット)](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/ioc)
    * サービスだけでなくViewModelもこれでインスタンス化できる。
    * サービス/ViewModelの登録は基本的に以下を用いる。
        * シングルトンなクラス: `AddSingleton<T>()`, `AddSingleton<I, C>()`
        * オブジェクト取得毎に新規作成(`new`)されるクラス: `AddTransient<T>()`, `AddTransient<I, C>`
    * 自前でシングルトンの制御を実装するのは手間がかかるので、これを使用することを推奨。
    * インスタンス化にDIを必要とする(=他のサービスや他のViewModelに依存する)ViewModelをDataContextに設定する場合は、添付プロパティを用いるとXAMLデザイナーで正しく表示させることができる。
        * 添付プロパティの実装: `SimpleCalendar.WPF/Views/Helpers/DataContextHelper.cs`
        * 添付プロパティの利用: `SimpleCalendar.WPF/MainWindow.xaml` / `SimpleCalendar.WPF/Views/CalendarMonthView.xaml`
            * XAMLデザイナーにViewModelを認識させるため、`d:DataContext="{d:DesignInstance Type=【ViewModelのクラス名】}"` も合わせて設定する。(添付プロパティでインスタンス化されるので`IsDesignTimeCreatable=True`は不要)
    * 登録するサービス/ViewModelはコンストラクタインジェクションを使うよう設計する。
        * インスタンス化に使用されるコンストラクタは、登録されたクラス群によって解決可能なもののうち、最も引数の多いものが選択される。
        * 引数の数が同じである解決可能なコンストラクタが複数ある場合は実行時エラーが発生する。
        * 特定のコンストラクタを使用することを明示的に指示したい場合は、`AddXXX()`の引数にインスタンス化処理を行うラムダ式(`Action<T>`)を渡す。

非同期プログラミング
--------------------

* [非同期 MVVM アプリケーションのパターン: データ バインド](https://learn.microsoft.com/ja-jp/archive/msdn-magazine/2014/march/async-programming-patterns-for-asynchronous-mvvm-applications-data-binding)
    * Taskを直接待つ(Wait/Result)のは、処理がブロックされるので禁止。(⇒ よくある誤り)
    * 「サービスは UI 非依存なので、待機のたびに ConfigureAwait(false) を使用します」(`ConfigureAwait(false)` is 何?)
* [初心者のためのTask.Run(), async/awaitの使い方](https://qiita.com/inew/items/0126270bca99883605de)
* [asyncの落とし穴Part3, async voidを避けるべき100億の理由](https://neue.cc/2013/10/10_429.html)
* [ReaderWriterLockSlimクラス](https://so-zou.jp/software/tech/programming/c-sharp/thread/reader-writer-lock-slim.htm)
    * [ReaderWriterLockクラス](https://so-zou.jp/software/tech/programming/c-sharp/thread/reader-writer-lock.htm)
        * 『特段の理由がなければReaderWriterLockSlimクラスを使用します。』
    * [ReaderWriterLockSlim and async\\await](https://stackoverflow.com/questions/19659387/readerwriterlockslim-and-async-await)
        * `ReaderWriterLockSlim`でロックを取得している間に`await`してはいけない。`ReaderWriterLockSlim`は現在のスレッドに依存しているので、await後別スレッドになってしまうと壊れる。`ConfigureAwait(true)`すれば問題ないかも?(未検証)
* [Async/Await - 非同期プログラミングのベスト プラクティス](https://learn.microsoft.com/ja-jp/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
    * `ConfigureAwait(false)`を使うと、`await`前後でコンテキストが維持されない = 元のスレッドに戻るとは限らない → `await`前後でViewの操作を行うと壊れる (UI非依存ならば問題ない)
[バインドしてるObservableCollectionを非UIスレッドから操作する](https://sourcechord.hatenablog.com/entry/2014/02/02/153725)

設定ファイルの扱い
------------------

* [アプリケーション設定の管理 (.NET)](https://learn.microsoft.com/ja-jp/visualstudio/ide/managing-application-settings-dotnet?view=vs-2022)
    * いろいろ試行錯誤してみたが、あくまでユーザーに不可視の情報を永続化するための仕組みであり、ユーザーが設定ファイルを直接変更するようなユースケースは想定していない模様。後者を実現するための標準的な方法は無さそうなので、自前で実装するかNuGetパッケージの導入を検討する。
        * こんな感じのパス名になる:
            * `C:\Users\vmi\AppData\Local\vmi.jp\SimpleCalendar.WPF_Url_wfyd44drf50k3ptum5onbl55enlyeje0\1.0.0.0\user.config`
        * アセンブリのバージョンが変わると格納先ディレクトリも変わるが、これは `Settings.Default.Upgrade()` でアップグレードできる模様。
            * 参考: [前バージョンの設定を引き継ぐ方法（C#用メモ）](https://among-ev.hatenadiary.org/entry/20110219/1298120888)
    * あと、WPFアプリ内では動作するが、xUnitで外部から呼び出すと正しく動かなかった。pack URIと類似の問題があるかも? (根拠なし)
* [\[C#/WPF\]Setting.settingを使用して、簡易的にアプリの設定値を保存/読出しする](https://qiita.com/tera1707/items/2ebc0e5c48dc5226f60c)
* ユーザー設定ファイル用のディレクトリ情報を取得して自前で処理する場合
    * [.NET TIPS Windowsのシステム・フォルダのパスを取得するには？](https://atmarkit.itmedia.co.jp/fdotnet/dotnettips/032spfolder/spfolder.html)
        * 古い記事だが、基本は変わらないはず。
        * [Environment クラス](https://learn.microsoft.com/ja-jp/dotnet/api/system.environment?view=net-8.0)
            * [Environment.GetFolderPath メソッド](https://learn.microsoft.com/ja-jp/dotnet/api/system.environment.getfolderpath?view=net-8.0)
            * [Environment.SpecialFolder 列挙型](https://learn.microsoft.com/ja-jp/dotnet/api/system.environment.specialfolder?view=net-8.0)
    * 初回起動時に設定ファイルをコピーしたい場合は、アプリのパスも取得する必要がある。
        * [【.Net】アプリのパスを取得する方法っていくつもあるけど何か違うの？【C#】](https://dokuro.moe/dot-net-what-is-difference-app-path-get-method/)

インストーラ作成
----------------

* ベストプラクティスについて、本資料執筆時点では結論出ず。
    * MSIX Packaging Tool: かなり余計なことをしてくれるので、シンプルにパッケージングしたいだけの用途には向かないように思われる。(ガチガチに統制された環境へのリリース向け?)
    * Microsoft Visual Studio Installer Project 2022: ちょっと動かしてみようと思ったがエラーが出たので棚上げ。
    * WiX toolset: 無料で使用可能なパッケージングツールで最もメジャーっぽい?
        * VS2022用のプラグインはV3だが、現時点での最新はV5。プラグインを使わず使用することを想定している?
        * アウトラインを把握するのにも時間がかかりそうなので、とりあえず保留。
    * ClickOnce: 最もシンプルにインストーラを作成可能? 得失については未調査。
        * プロジェクトを右クリック →「発行」で使用可能。「公開」ダイアログで「ClickOnce」を選択する。
        * 「発行」で公開ダイアログが表示されない場合は「公開」タブの「発行プロファイルの追加」をクリックする。
* 暫定的に、フォルダに「発行」してzipで固める方針に決定。

Visual Studio 2022
------------------

* VS拡張機能
    * [XAMLのコード整形を簡単にする方法【Visual Studio】](https://lifetime-engineer.com/xaml-code-formatting/)
* [MSBuild の予約済みおよび既知のプロパティ](https://learn.microsoft.com/ja-jp/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2022)
    * [MSBuild reserved and well-known properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2022)

オープンソースのアイコン集
--------------------------

* [iconify.design](https://icon-sets.iconify.design/)
    * OSSライセンスのアイコンを集めたもの。

ヘルプの取り扱い方針
--------------------

* HTMLで書いて標準ブラウザに表示させる。
    * 利点: シンプルでメモリも食わない。
    * 問題点: 別アプリでの表示となってしまう。短いテキストだとややおおげさ。
* HTMLで書いて内蔵ブラウザに表示させる。
    * 選択肢1: WebBrowserコントロールを使用する。
        * 利点: 同一アプリ内での表示が可能。
        * 問題点
            * メモリを余計に使用する可能性がある。
            * リンクがあると外部サイトにアクセスしてしまう可能性がある。
            * ブラウザエンジンが旧IE/旧Edgeの可能性がある。(未検証)
    * 選択肢2: [Microsoft Edge WebView2](https://learn.microsoft.com/ja-jp/microsoft-edge/webview2/) を使用する。
        * 利点: 同一アプリ内での表示が可能。最新のブラウザエンジンが使える。
        * 問題点
            * メモリを余計に使用する可能性がある。
            * リンクがあると外部サイトにアクセスしてしまう可能性がある。
            * 別途nupkgを追加する必要がある。
* XAMLファイルにFlowDocumentScrollViewerコントロールを配置し、その中にFlowDocumentで直接書く。
    * 利点: シンプルで比較的メモリも食わず、アプリに統合される。また、Visual StudioのXAMLデザイナー上に直接表示可能。
    * 問題点: 単一ドキュメント向け。複数のドキュメントを行き来するようなナビゲーションはできない。
* XAMLファイルにFlowDocumentScrollViewerコントロールを配置し、FlowDocumentを別ファイルからXamlReaderで読み出したものを埋め込む。
    * 利点: シンプルで比較的メモリも食わず、アプリに統合される。
    * 問題点
        * Visual StudioのXAMLデザイナーでは表示できないため、実行してみないと挙動がわからない。
        * ヘルプ画面のナビゲーションを行う場合、ナビゲーション用の仕組みを作り込む必要がある。

以上
