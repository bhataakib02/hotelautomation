namespace StayFlow.Api.Middleware;
public class ErrorHandlingMiddleware(RequestDelegate next,ILogger<ErrorHandlingMiddleware> log){
 public async Task Invoke(HttpContext c){try{await next(c);}catch(Exception e){
  var (code,msg)=e switch{ArgumentException=>(400,e.Message),KeyNotFoundException=>(404,e.Message),InvalidOperationException=>(409,e.Message),_=>(500,"Something went wrong. Please try again.")};
  if(code==500)log.LogError(e,"Unhandled error");c.Response.StatusCode=code;await c.Response.WriteAsJsonAsync(new{message=msg});}}}
