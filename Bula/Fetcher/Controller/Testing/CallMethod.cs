// Buddy Fetcher: simple RSS-fetcher/aggregator.
// Copyright (c) 2020 Buddy Lancer. All rights reserved.
// Author - Buddy Lancer <http://www.buddylancer.com>.
// Licensed under the MIT license.

namespace Bula.Fetcher.Controller.Testing {
    using System;

    using Bula.Fetcher;
    using Bula.Fetcher.Controller;
    using System.Collections;
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
            Request.Initialize();
    		Request.ExtractAllVars();

            // Check security code
    		if (!Request.Contains("code"))
    			Response.End("Code is required!");
    		var code = Request.Get("code");
    		if (!EQ(code, Config.SECURITY_CODE))
    			Response.End("Incorrect code!");

    		// Check package
            if (!Request.Contains("package"))
    			Response.End("Package is required!");
    		var package = Request.Get("package");
    		if (BLANK(package))
    			Response.End("Empty package!");
            String[] package_chunks = Strings.Split("-", package);
            for (int n = 0; n < SIZE(package_chunks); n++)
                package_chunks[n] = Strings.FirstCharToUpper(package_chunks[n]);
    		package = Strings.Join("/", package_chunks);

    		// Check class
            if (!Request.Contains("class"))
    			Response.End("Class is required!");
    		var className = Request.Get("class");
    		if (BLANK(className))
    			Response.End("Empty class!");

    		// Check method
            if (!Request.Contains("method"))
    			Response.End("Method is required!");
    		var method = Request.Get("method");
    		if (BLANK(method))
    			Response.End("Empty method!");

    		// Fill array with parameters
            var count = 0;
    		var pars = new ArrayList();
    		for (int n = 1; n <= 6; n++) {
    			var par_name = CAT("par", n);
    			if (!Request.Contains(par_name))
    				break;
    			var par_value = Request.Get(par_name);
    			if (EQ(par_value, "_"))
    				par_value = "";
    			//pars_array[] = par_value;
                pars.Add(par_value);
    			count++;
    		}

            var buffer = (String)null;
            var result = (Object)null;

            var full_class = CAT(package, "/", className);

    		full_class = Strings.Replace("/", ".", full_class);
            method = Strings.FirstCharToUpper(method);
            result = Internal.CallMethod(full_class, new ArrayList(), method, pars);

            if (result == null)
                buffer = "NULL";
            else if (result is DataSet)
                buffer = ((DataSet)result).ToXml();
            else
                buffer = STR(result);
            Response.Write(buffer);
    	}
    }
}