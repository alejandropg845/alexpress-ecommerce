using Azure.AI.ContentSafety;
using Azure;

namespace API.Services.Secondary
{
    public static class ContentSafetyService
    {
        public static async Task<(bool IsExplicitImage, bool IsExplicitDescription, bool IsExplicitTitle)> VerifyContentAsync(this List<string> imagesUrls, string cs_endpoint,
            string cs_key, string description, string title)
        {

            var byteArraysList = new List<byte[]>();

            var http = new HttpClient();

            var imageTasks = imagesUrls.Select(async image =>
            await http.GetByteArrayAsync(image));
            


            var bytesArraysList = (await Task.WhenAll(imageTasks))
                .Where(bytes => bytes != null).ToList();

            var contentSafetyClient = new ContentSafetyClient
                (
                    new Uri(cs_endpoint),
                    new AzureKeyCredential(cs_key)
                );

            var blocklistClient = new BlocklistClient
                (
                    new Uri(cs_endpoint),
                    new AzureKeyCredential(cs_key)
                );

            var analyzeImageTasks = bytesArraysList.Select(async imageBytes =>
            {
                var imageData = BinaryData.FromBytes(imageBytes);

                var response = await contentSafetyClient.AnalyzeImageAsync(imageData);
                return ImageAnalysisResult(response.Value.CategoriesAnalysis);
            });

            var imageAnalysisResults = await Task.WhenAll(analyzeImageTasks);


            bool isExplicitDescription;
            bool isExplicitTitle;

            var descriptionResponse = await contentSafetyClient.AnalyzeTextAsync(description);

            var titleResponse = await contentSafetyClient.AnalyzeTextAsync
                (title);

            isExplicitDescription = DescriptionAnalysisResult(
                descriptionResponse.Value.CategoriesAnalysis);
            isExplicitTitle = DescriptionAnalysisResult(
                titleResponse.Value.CategoriesAnalysis);

            bool isExplicitImage = imageAnalysisResults.Any(result => result);

            return (isExplicitImage, isExplicitDescription, isExplicitTitle);


        }

        public static bool ImageAnalysisResult(IReadOnlyList<ImageCategoriesAnalysis> r)
        {
            bool result = r.Any(a =>
            a.Category == ImageCategory.Violence && a.Severity > 2 ||
            a.Category == ImageCategory.SelfHarm && a.Severity > 2 ||
            a.Category == ImageCategory.Hate && a.Severity > 2 ||
            a.Category == ImageCategory.Sexual && a.Severity > 2);

            return result;

        }

        public static bool DescriptionAnalysisResult(IReadOnlyList<TextCategoriesAnalysis> r)
        {

            return r.Any(a =>
            a.Category == TextCategory.Violence && a.Severity != 0 ||
            a.Category == TextCategory.SelfHarm && a.Severity != 0 ||
            a.Category == TextCategory.Hate && a.Severity != 0 ||
            a.Category == TextCategory.Sexual && a.Severity != 0);

        }


    }
}
