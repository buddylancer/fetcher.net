# fetcher.net
Buddy Fetcher: Simple RSS fetcher/aggregator (.NET/MySQL).

Project features:
- collecting remote items (news, jobs etc) from a number of external websites
- using standard input RSS-feeds or custom parsing
- items fetcher can be run as frequently as needed (through scheduled task)
- separate functionality for operations with DB (data access layer)
- filtering items by categories (any number of categories)
- generate output RSS-feeds (filtered by categories also)
- caching logic for input RSS-feeds, web pages and output RSS-feeds
- mobile version is configured in View section (just through separate CSS)
- classic MVC (Model/View/Controller) implementation (own development)
- simple REST API for extracting collected data
- custom rules for item processing (shrink, cut, replace, extract etc)
- custom character mappings for item processing
- own testing framework (universal for PHP, .NET [Core] and Java versions)

Test scripts are located in local/tests folder.

This product was ported from original PHP version using own convertor.
The convertor is written in C# and using ~150 regular expressions.
Most of functionality (~90%) is auto-ported, the remaining is manually implemented.

Websites powered by Buddy Fetcher:
- 1001 Freelance Projects (http://www.1001freelanceprojects.com)
- 1001 Freelance Projects / RU (http://www.1001freelanceprojects.ru)
- 1001 Remote Jobs (http://www.1001remotejobs.com)
- 1001 Remote Jobs / RU (http://www.1001remotejobs.ru)

Other versions of Buddy Fetcher:
- PHP version - http://github.com/buddylancer/fetcher.php
- .NET Core version - http://github.com/buddylancer/fetcher.net.core
- Java version - http://github.com/buddylancer/fetcher.java

