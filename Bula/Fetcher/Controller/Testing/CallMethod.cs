namespace Bula.Fetcher.Controller.Testing {
    using System;

    using Bula.Fetcher;
    using System.Collections;
    using Bula.Objects;
    using Bula.Model;

    public class CallMethod : Bula.Meta {
    	public static void Execute() {
            Request.Initialize();
    		Request.ExtractAllVars();

    		if (!Request.Contains("code"))
    			STOP("Code is required!");
    		var code = Request.Get("code");
    		if (!EQ(code, Config.SECURITY_CODE))
    			STOP("Incorrect code!");

    		if (!Request.Contains("package"))
    			STOP("Package is required!");
    		var package = Request.Get("package");
    		if (BLANK(package))
    			STOP("Empty package!");
            String[] package_chunks = Strings.Split("-", package);
            for (int n = 0; n < SIZE(package_chunks); n++)
                package_chunks[n] = Strings.FirstCharToUpper(package_chunks[n]);
    		package = Strings.Join("/", package_chunks);

    		if (!Request.Contains("class"))
    			STOP("Class is required!");
    		var className = Request.Get("class");
    		if (BLANK(className))
    			STOP("Empty class!");

    		if (!Request.Contains("method"))
    			STOP("Method is required!");
    		var method = Request.Get("method");
    		if (BLANK(method))
    			STOP("Empty method!");

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
            var result = (DataSet)null;

            var full_class = CAT(package, "/", className);

    		full_class = Strings.Replace("/", ".", full_class);
            method = Strings.FirstCharToUpper(method);
            result = (DataSet)Util.CallMethod(full_class, method, pars);

            buffer = result == null ? "NULL" : result.Serialize();
            Response.Write(buffer);
    	}
    }

}