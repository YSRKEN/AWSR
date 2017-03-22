# AWSR
Air War Simulator Returns for Kantai Collection

# 概要
- 艦これにおける航空戦をシミュレートして、次のような結果を返します。
  - 基地航空隊および本隊と敵艦隊との交戦による、各交戦状態の確率
  - 空母の各スロットにおける全滅率、および攻撃可能な艦載機が全滅する確率(棒立ち率)
  - 艦娘・基地航空隊・深海棲艦における各スロットの残存分布のグラフ表示
- このソフトウェアを作成するため、次のソフトウェアを参考にしました。
  - [pokopii](https://twitter.com/galpokopii)氏の「基地航空隊シミュレーション」
  - [さかさま](https://twitter.com/mif_syo)氏の「[航空戦シミュレーション](http://ux.getuploader.com/airwarfaresimulation/download/31/dist.zip)」

# 操作説明
- **見たままです**

# 真面目な操作説明
- **利用可能な入出力形式は、sampleフォルダ内のsample.mdを参照して下さい**
- 自艦隊は、テキストボックスにテキストをコピペしてください
 - 「デッキビルダーで開く」をクリックすると、デッキビルダーのサイトが立ち上がります
 - 「デッキビルダーで開く」ボタンを右クリックすると、デッキビルダー形式か独自形式かで内容をコピーできます
 - 艦隊数は、「入力通り」だと入力した通りの艦隊数(最大2艦隊)になります
- 基地航空隊・敵艦隊は、テキストボックスにコピペするか、「ファイルから読み込み」から読み込んでください
- 「静的解析」の項目では、艦隊の情報や制空力を試算することができます
- 「動的解析」の項目では、特定の制空状態となる確率や棒立ち率(全滅率)がダイアログ表示されます
 - また、動的解析すると、艦載機の残存数の積み上げ折れ線グラフを表示できます
 - 「テキストをコピー」「画像をコピー」ボタンで表示している内容をクリップボードにコピーできます

# 注意
- 計算精度については保証しません
- 各種対空カットインの発動率はwikiとかを見ながら適当に設定しています

# サンプルデータについて
　sampleフォルダ内のsample.mdを参照してください

# ライセンス

|パート|作成者|ライセンス|
|------|------|----------|
|コード本体|YSR|MIT License|
|乱数生成コード|Rei Hobara|MIT License|

# 更新履歴

## Ver.1.1.0
- 乱数生成アルゴリズムをSFMTに変更
- 艦載機を搭載してない艦は動的解析の結果に表示しないようにした
- チェックボックスで表示するグラフを切り替えられるようにした
- 自艦隊について、デッキビルダー形式と独自形式とを変換＆コピーできるようにした
- 動的解析の結果をクリップボードにコピーできるようにした

## Ver.1.0.0 2017/3/19
- 初版
