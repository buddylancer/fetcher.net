// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020-2021 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Testing {
    using System;
    using System.Collections;

    using Bula.Fetcher;
    using Bula.Fetcher.Controller;
    using Bula.Objects;
    using Bula.Model;

    /// <summary>
    /// Logic for remote method invocation.
    /// </summary>
    public class CallMethod : Page {
        /// <summary>
        /// Public default constructor.
        /// </summary>
        /// <param name="context">Context instance.</param>
        public CallMethod(Context context) : base(context) { }

        /// Execute method using parameters from request. 
        public override void Execute() {
            //this.context.Request.Initialize();
            this.context.Request.ExtractAllVars();

            this.context.Response.WriteHeader("Content-type", "text/html; charset=UTF-8");

            // Check security code
            if (!this.context.Request.Contains("code")) {
                this.context.Response.End("Code is required!");
                return;
            }
            var code = this.context.Request["code"];
            if (!EQ(code, Config.SECURITY_CODE)) {
                this.context.Response.End("Incorrect code!");
                return;
            }

            // Check package
            if (!this.context.Request.Contains("package")) {
                this.context.Response.End("Package is required!");
                return;
            }
            var package = this.context.Request["package"];
            if (BLANK(package)) {
                this.context.Response.End("Empty package!");
                return;
            }
            String[] packageChunks = Strings.Split("-", package);
            for (int n = 0; n < SIZE(packageChunks); n++)
                packageChunks[n] = Strings.FirstCharToUpper(packageChunks[n]);
            package = Strings.Join("/", packageChunks);

            // Check class
            if (!this.context.Request.Contains("class")) {
                this.context.Response.End("Class is required!");
                return;
            }
            var className = this.context.Request["class"];
            if (BLANK(className)) {
                this.context.Response.End("Empty class!");
                return;
            }

            // Check method
            if (!this.context.Request.Contains("method")) {
                this.context.Response.End("Method is required!");
                return;
            }
            var method = this.context.Request["method"];
            if (BLANK(method)) {
                this.context.Response.End("Empty method!");
                return;
            }

            // Fill array with parameters
            var count = 0;
            var pars = new TArrayList();
            for (int n = 1; n <= 6; n++) {
                var parName = CAT("par", n);
                if (!this.context.Request.Contains(parName))
                    break;
                var parValue = this.context.Request[parName];
                if (EQ(parValue, "_"))
                    parValue = "";
                //parsArray[] = parValue;
                pars.Add(parValue);
                count++;
            }

            var buffer = (String)null;
            var result = (Object)null;

            var fullClass = CAT(package, "/", className);

            fullClass = Strings.Replace("/", ".", fullClass);
            method = Strings.FirstCharToUpper(method);
            TArrayList pars0 = new TArrayList(new Object[] { this.context.Connection });
            result = Bula.Internal.CallMethod(fullClass, pars0, method, pars);

            if (result == null)
                buffer = "NULL";
            else if (result is DataSet)
                buffer = ((DataSet)result).ToXml(EOL);
            else
                buffer = STR(result);
            this.context.Response.Write(buffer);
            this.context.Response.End();
        }
    }
}