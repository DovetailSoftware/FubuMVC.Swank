﻿$(function () {

    var moduleTemplate = Handlebars.compile($('#swank-module-template').html());
    var resourceTemplate = Handlebars.compile($('#swank-resource-template').html());

    Handlebars.registerPartial("headers", $("#swank-headers-template").html());
    Handlebars.registerPartial("urlParameters", $("#swank-url-parameters-template").html());
    Handlebars.registerPartial("querystring", $("#swank-querystring-template").html());
    Handlebars.registerPartial("jsonDataFormat", $("#swank-data-json-template").html().flatten());
    Handlebars.registerPartial("xmlDataFormat", $("#swank-data-xml-template").html().flatten());
    Handlebars.registerPartial("dataDescription", $("#swank-data-description-template").html());
    Handlebars.registerPartial("data", $("#swank-data-template").html());
    Handlebars.registerPartial("codeExamples", $("#swank-code-examples-template").html());
    Handlebars.registerPartial("statusCodes", $("#swank-status-codes-template").html());
    Handlebars.registerPartial("options", $("#swank-options-template").html());
    
    var render = function (id) {
            
        $('.nav').find('li').removeClass('active');
        
        var content = $('#content');
        content.empty();
        
        if (!id) {
            content.html(Swank.Comments);
            return;
        }
        
        var idParts = id.split('@', 2);
        var moduleName = idParts[0];
        var resourceName = idParts[1];

        var module = Swank.Modules.filter(function (x) { return x.Name === moduleName; })[0];
        var resource = module.Resources.filter(function (x) { return x.Name === resourceName; })[0];

        $('.nav').find("li[data-module='" + moduleName + "']").addClass('active');
        
        if (module && !resource) {
            content.html(moduleTemplate(module));
        } else if (resource) {
            content.html(resourceTemplate(resource));
            $('.endpoint-header').click(function () {
                var header = $(this);
                header.next(".endpoint-body").slideToggle(500);
                header.find('.expand-toggle').toggleClass('icon-chevron-left');
                header.find('.expand-toggle').toggleClass('icon-chevron-down');
            });
            
            $('.show-json').click(function () {
                $('.show-json').addClass('active');
                $('.show-xml').removeClass('active');
                $(content).find(".json").show();
                $(content).find(".xml").hide();
                $.cookie('FormatPreference', 'json');
                return false;
            });
            
            $('.show-xml').click(function () {
                $('.show-xml').addClass('active');
                $('.show-json').removeClass('active');
                $(content).find(".xml").show();
                $(content).find(".json").hide();
                $.cookie('FormatPreference', 'xml');
                return false;
            });

            $(window).resize(function () { $('.code-raw-container').remove(); });

            function createCodeBox(top, left, width, height, code) {
                var container = $('<div class="sample-code-raw-container" style="' +
                    'top: ' + (top) + 'px;' +
                    'left: ' + (left) + 'px;' +
                    'width: ' + (width) + 'px;' +
                    'height: ' + (height) + 'px;' +
                    '"><textarea class="sample-code-raw">' + code + '</textarea></div>').appendTo(content);
                $(container).find('textarea').select().blur(function (x) { $(x.target).parent().remove(); });
            }

            $('td.sample-code').click(function () {
                var table = $(this).closest("table");
                var code = table.find('.sample-code.json:visible, .sample-code.xml:visible')
                    .map(function () { return $(this).text(); })
                    .get().join("\r\n");
                var top = table.find('.sample-code:visible:first');
                var bottom = table.find('.sample-code:visible:last');
                createCodeBox(
                    top.offset().top + 1,
                    top.offset().left + 1,
                    top.outerWidth() - 2,
                    (bottom.offset().top - top.offset().top) + bottom.outerHeight() - 2,
                    code);
            });

            $('.code-example').click(function () {
                createCodeBox(
                    this.offsetTop,
                    this.offsetLeft,
                    this.offsetWidth,
                    this.offsetHeight,
                    $(this).text());
            });
        } else {
            content.html('No module our resource specified.');
        }
    };

    Handlebars.registerHelper('methodColor', function (context) {
        if (!context) return 'blue';
        switch (context.toLowerCase()) {
            case 'get': return 'blue';
            case 'post': return 'green';
            case 'put': return 'yellow';
            case 'update': return 'yellow';
            case 'delete': return 'red';
            default: return 'blue';
        }
    });
    
    Handlebars.registerHelper('formatUrl', function (context) {
        return context.replace(/(\{.*?\})/g, '<span class="highlight-text"><b>$1</b></span>');
    });

    Handlebars.registerHelper('either', function (value1, value2, options) {
        if (value1 || value2) {
            return options.fn(this);
        }
        return options.inverse(this);
    });

    var initialize = function () {

        var getHash = function() { return window.location.hash.replace(/^#/, ''); };
        
        if (!Swank.Comments && !getHash() &&
            (Swank.Modules && Swank.Modules.length > 0))
            window.location.hash = Swank.Modules[0].Name + '@' + Swank.Modules[0].Resources[0].Name;

        $(window).bind('hashchange', function () { render(getHash()); });
    
        if (getHash()) $(window).trigger('hashchange');
    };

    initialize();
});