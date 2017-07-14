using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SingletonText : Singleton<SingletonText>
{
    public TextToSpeechManager tts; //英語読み上げ用TTS
    public AudioSource source;      //Azureからの返却ファイルを読み上げるためのソース

    private string ja;              //日本語文字列保持用変数
    private string en;              //英語文字列保持用変数

    private string bingTokenUrl = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken"; //トークン取得用URL
    private string bingAPIUrl = "https://speech.platform.bing.com/synthesize";             //API呼び出し用URL

    /*
     *日本語説明文をテキストエリアに表示。同時にローカル保持。
     */
    public void SetJaText(string s)
    {
        gameObject.GetComponent<Text>().text = s;
        ja = s;
    }

    /*
     * 英語説明文をローカル保持
     */
    public void SetEnText(string s)
    {
        en = s;
    }

    /*
     * 日本語ボタン押下時の発話処理
     */
    public void ClickButton_Ja()
    {

        var bingHeaders = new Dictionary<string, string>() {
            { "Ocp-Apim-Subscription-Key", "API_KEY" } //Bing Speech APIのAPIキーを入力
        };

        StartCoroutine(HttpPost(bingTokenUrl, bingHeaders, ja));
    }

    /*
     * HTTP POST リクエスト
     */
    IEnumerator HttpPost(string url, Dictionary<string, string> header, string japaneseString)
    {
        //Tokenの取得
        WWW www = new WWW(url, new byte[1], header); //POSTで投げたいのでダミーとして空のバイト配列を入れている
        yield return www;
        string bingToken = www.text;

        //SpeechAPIの呼び出し
        var headers = new Dictionary<string, string>() {
            { "Content-Type", "application/ssml+xml" },
            { "X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm" },
            { "Authorization", "Bearer "+ bingToken},
            { "X-Search-AppId", "07D3234E49CE426DAA29772419F436CA" },
            { "X-Search-ClientID", "1ECFAE91408841A480F00935DC390960" },
            { "User-Agent", "TTSClient" }
        };

        string ssml = GenerateSsml("ja-JP", "Female", "Microsoft Server Speech Text to Speech Voice (ja-JP, Ayumi, Apollo)", japaneseString);
        byte[] bytes = Encoding.UTF8.GetBytes(ssml);

        WWW wwwAPI = new WWW(bingAPIUrl, bytes, headers);
        yield return wwwAPI;

        source.clip = wwwAPI.GetAudioClip(true, true, AudioType.WAV);
        source.Play();
    }

    /*
     * SSML生成
     */
    private string GenerateSsml(string locale, string gender, string name, string text)
    {
        var ssmlDoc = new XDocument(
                          new XElement("speak",
                              new XAttribute("version", "1.0"),
                              new XAttribute(XNamespace.Xml + "lang", "ja-JP"),
                              new XElement("voice",
                                  new XAttribute(XNamespace.Xml + "lang", locale),
                                  new XAttribute(XNamespace.Xml + "gender", gender),
                                  new XAttribute("name", name),
                                  text)));
        return ssmlDoc.ToString();
    }

    /*
     * 英語ボタン押下時の発話処理
     */
    public void ClickButton_En()
    {
        tts.SpeakText(en); //英語の場合はこの一文だけ。あらかじめ保持しておいた文字列をSpeakTextに渡す。
    }
}