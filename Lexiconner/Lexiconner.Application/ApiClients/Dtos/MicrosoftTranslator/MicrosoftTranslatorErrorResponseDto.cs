namespace Lexiconner.Application.ApiClients.Dtos.MicrosoftTranslator
{
    public class MicrosoftTranslatorErrorResponseDto
    {
        public ErrorDto Error { get; set; }

        public class ErrorDto
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }
}
