namespace GestaoFacil.Server.Responses
{
    public class ResponseHelper
    {
        public static ResponseModel<T> Sucesso<T>(T? dados, string mensagem = "Operação realizada com sucesso.")
        {
            return new ResponseModel<T> { Dados = dados, Mensagem = mensagem, Status = true };
        }

        public static ResponseModel<T> Falha<T>(string mensagem, T? dados = default)
        {
            return new ResponseModel<T> { Dados = dados, Mensagem = mensagem, Status = false };
        }
    }
}
