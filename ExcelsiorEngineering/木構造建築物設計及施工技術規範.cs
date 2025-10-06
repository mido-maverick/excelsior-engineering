namespace ExcelsiorEngineering;

/// <summary>
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>
///             <see href="https://www.nlma.gov.tw/ch/legislation/regsearch/960">
///                 木構造建築物設計及施工技術規範 | 內政部國土管理署全球資訊網
///             </see>
///         </item>
///         <item>
///             <see href="https://glrs.moi.gov.tw/LawContent.aspx?id=FL025530">
///                 內政部主管法規查詢系統-法規內容-木構造建築物設計及施工技術規範
///             </see>
///         </item>
///     </list>
/// </remarks>
public static class 木構造建築物設計及施工技術規範
{
    /// <remarks>
    ///     <see href="https://www.nlma.gov.tw/uploads/files/c3069fb4ce95f330816455e1f10e5b11.pdf">
    ///         木構造建築物設計及施工技術規範 第四章 材料及容許應力
    ///     </see>
    /// </remarks>
    public static class 材料及容許應力
    {
        public enum 樹種
        {
            // 針葉樹Ⅰ~Ⅳ類
            花旗松, 俄國落葉松,
            羅漢柏, 扁柏, 羅森檜, 硬木南方松,
            赤松, 黑松, 落葉松, 鐵杉, 北美鐵杉, 軟木南方松, 世界爺,
            冷杉, 蝦夷松, 椵松, 朝鮮松, 柳杉, 西部側柏, 雲杉, 杉木, 台灣杉, 放射松,

            // 闊葉樹Ⅰ~Ⅲ類
            樫木,
            栗木, 櫟木, 山毛櫸, 櫸木, 油脂木, 冰片樹, 硬槭木,
            柳桉
        }
        public enum 針闊葉樹別 { 針葉, 闊葉 }
        public enum 木材類別 { Ⅰ = 1, Ⅱ, Ⅲ, Ⅳ }
        public enum 木材等級 { 普通, 上等 }
    }
}