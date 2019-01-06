namespace Track

module Authentication =

    open Settings
    open Microsoft.AspNetCore.Authentication
    open Saturn
    open Microsoft.AspNetCore.Builder
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.AspNetCore.Authentication.Cookies
    open Microsoft.AspNetCore.Authentication.OpenIdConnect
    open FSharp.Control.Tasks
    open System.Threading.Tasks
    open System

    let private addCookie state (c : AuthenticationBuilder) =
        if not state.CookiesAlreadyAdded
            then c.AddCookie() |> ignore

    type ApplicationBuilder with
        ///Enables OpenId authentication with custom configuration
        [<CustomOperation("use_open_id_auth_with_config")>]
        member __.UseOpenIdAuthWithConfig(state: ApplicationState, (config : System.Action<OpenIdConnectOptions>)) =
          let middleware (app : IApplicationBuilder) =
            app.UseAuthentication()

          let service (s : IServiceCollection) =
            let c = s.AddAuthentication(fun cfg ->
              cfg.DefaultScheme <- CookieAuthenticationDefaults.AuthenticationScheme
              cfg.DefaultChallengeScheme <- OpenIdConnectDefaults.AuthenticationScheme
              cfg.DefaultSignInScheme <- CookieAuthenticationDefaults.AuthenticationScheme)
            addCookie state c
            c.AddOpenIdConnect("OpenIdConnect",config) |> ignore
            s

          { state with
              ServicesConfig = service::state.ServicesConfig
              AppConfigs = middleware::state.AppConfigs
              CookiesAlreadyAdded = true }

    let openIdConfig = 
        let events =  new OpenIdConnectEvents()
        events.OnRedirectToIdentityProviderForSignOut <- fun context ->
            let logoutUri =
                let logoutUri = sprintf "https://%s/v2/logout?client_id=%s" Setting.Auth.Issuer Setting.Auth.ClientId

                match context.Properties.RedirectUri with
                | x when System.String.IsNullOrEmpty x -> logoutUri
                | x ->
                    let postLogoutUri =
                        if x.StartsWith("/")
                            then
                                let request = context.Request
                                request.Scheme + "://" + request.Host.ToString() + request.PathBase + x
                        else x
                    logoutUri + (sprintf "&returnTo=%s" <| Uri.EscapeDataString(postLogoutUri))

            context.Response.Redirect(logoutUri)
            context.HandleResponse()
            Task.CompletedTask

        let options = fun (opt : OpenIdConnectOptions) ->
            opt.ClientId <- Setting.Auth.ClientId
            opt.Authority <- "https://" + Setting.Auth.Issuer
            opt.UseTokenLifetime <- true
            opt.CallbackPath <- Microsoft.AspNetCore.Http.PathString("/signin-auth0")    
            opt.ResponseType <- "code"
            opt.ClaimsIssuer <- "Auth0"
            opt.Events <- events

        new System.Action<OpenIdConnectOptions>(options)

