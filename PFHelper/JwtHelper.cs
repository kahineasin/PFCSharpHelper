using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using JWT.Exceptions;

namespace Perfect
{
    public class JwtHelper
    {
        static IJwtAlgorithm algorithm = new HMACSHA256Algorithm();//HMACSHA256加密  
        static IJsonSerializer serializer = new JsonNetSerializer();//序列化和反序列  
        static IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();//Base64编解码  
        static IDateTimeProvider provider = new UtcDateTimeProvider();//UTC时间获取
        //const string secret = "9f0b293e2a66003d8ab39a6f26431be3798cdd1912975be8a42b9be6c552248c";//服务端
        public static string CreateJWT(Dictionary<string, object> payload, string secret)
        {
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            return encoder.Encode(payload, secret);
        }
        public static bool ValidateJWT(string token, out string payload, out string message, string secret)
        {
            bool isValidted = false;
            payload = "";
            try
            {
                IJwtValidator validator = new JwtValidator(serializer, provider);//用于验证JWT的类
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);//用于解析JWT的类
                payload = decoder.Decode(token, secret, verify: true);
                isValidted = true;
                message = "验证成功";
            }
            catch (TokenExpiredException)//当前时间大于负载过期时间（负荷中的exp），会引发Token过期异常
            {
                message = "过期了！";
            }
            catch (SignatureVerificationException)//如果签名不匹配，引发签名验证异常
            {
                message = "签名错误！";
            }
            return isValidted;
        }
    }
}
