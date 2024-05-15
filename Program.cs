using Aspose.Pdf.Text;

namespace AsposeLeakReproduce
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string receiptTextDocument = "\r\n\r\n                                                 S00000/YIKIKKEMER İZMİR ŞUBESİ                     \r\n\r\n         TestOtomasyonikcdf Dokunmaikcdf       İŞLEM        : HESAP AÇ                              \r\n   44511111111                                TARİH        : 15.05.2024 11:56:07                   \r\n   GERS MAH. HACI YER CAD. SUN SET STES K B    VALÖR        : 16.05.2024                            \r\n   TCKN/VKN:  58308111111                      İŞLEM NO     : 111100011171111                      \r\n\r\n\r\n   IBAN  : TR01 0001 1001 1100 9119 1111 11                                                         \r\n   Ürün Adı : TL Vadesiz Hesap                                                                      \r\n                                                                                                    \r\n   Tutar : 1.000,00 TL                                                                              \r\n                                                                                                    \r\n                                                                                                    \r\n   Rumuz : TestOtomasyon                                                                            \r\n                                                                                                    \r\n   Yukarıda detay bilgileri verilen hesap açılış işlemi yapılmıştır.                                \r\n   Açıklama     :                                                                                   \r\n                                                                                                    \r\n                                                                                                    \r\n                                                                                                    \r\n   Adına işlem Yapan: Kendisi                                                                       \r\n                                                                                                    \r\n\r\n                                                                                                    \r\n                                                                                                    \r\n\r\n   İşlem                        Onaylayan                                                           \r\n   VB22259                      TEST                                                                \r\n   1.000,00 TL tahsil edildi.                                                                       \r\n\r\n";
            int normalReceiptCountOnTheLastPage = 0;

            using (var bgImageStream = new MemoryStream())
            {
                using (FileStream fileStream = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"files\\bgImage"), FileMode.Open))
                {
                    fileStream.CopyTo(bgImageStream);
                }

                Aspose.Pdf.Page currentPage = null;
                Aspose.Pdf.Document currentPdfDocument = new Aspose.Pdf.Document();
                currentPage = currentPdfDocument.Pages.Add();
                currentPage.PageInfo.Margin = new Aspose.Pdf.MarginInfo(0, 0, 0, 0);

                float floatingBoxTop = (float)(normalReceiptCountOnTheLastPage % 2 == 0 ? 21 : (currentPage.PageInfo.Height / 2) + 21);
                float floatingBoxLeft = 0;
                float floatingBoxWidth = (float)currentPage.PageInfo.Width - floatingBoxLeft;
                float floatingBoxHeight = (float)((currentPage.PageInfo.Height / 2) - 21);
                normalReceiptCountOnTheLastPage++;

                string[] stringSeparators = new string[] { "\r\n" };

                string[] textDocument;

                textDocument = receiptTextDocument.Split(stringSeparators, StringSplitOptions.None);

                for (int i = 0; i < textDocument.Length; i++)
                {
                    textDocument[i] = textDocument[i].TrimEnd();
                }
                var listText = new List<string>(textDocument);

                int count = listText.Count;

                for (int i = count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(listText[i]) || string.IsNullOrEmpty(listText[i].TrimEnd()))
                    {
                        listText.RemoveAt(i);
                    }
                    else
                    {
                        break;
                    }
                }

                string text = "";
                foreach (string s in listText)
                {
                    text += s + "\r\n";
                }

                TextFragment textFragment = new TextFragment(text);
                textFragment.Position = new Position(floatingBoxTop, floatingBoxLeft);

                textFragment.TextState.FontSize = 10.5f;
                textFragment.TextState.Font = FontRepository.FindFont("CourierNew");

                Aspose.Pdf.FloatingBox floatingBox = new Aspose.Pdf.FloatingBox(floatingBoxWidth, floatingBoxHeight);
                floatingBox.ZIndex = 10;
                floatingBox.Left = floatingBoxLeft;
                floatingBox.Top = floatingBoxTop;
                floatingBox.Paragraphs.Add(textFragment);
                currentPage.Paragraphs.Add(floatingBox);  // Memory leak fixed after removing this line...

                Aspose.Pdf.Image asposeImage = new Aspose.Pdf.Image();
                asposeImage.ImageStream = bgImageStream;

                asposeImage.FixHeight = currentPage.PageInfo.Height / 2;
                asposeImage.FixWidth = currentPage.PageInfo.Width;

                asposeImage.ZIndex = -10;

                currentPage.Paragraphs.Add(asposeImage);

                using (MemoryStream msResult = new MemoryStream())
                {
                    // SaveXml generates document successfully without performance issue
                    // currentPdfDocument.SaveXml(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"files\\result.xml"));

                    // here the memory leak starts ( leak fixed after remove line currentPage.Paragraphs.Add(floatingBox) )
                    currentPdfDocument.Save(msResult, Aspose.Pdf.SaveFormat.Pdf);
                }

            }
        }
    }
}
