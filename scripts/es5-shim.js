(function(n){"function"==typeof define?define(n):"function"==typeof YUI?YUI.add("es5",n):n()})(function(){function n(a){try{return Object.defineProperty(a,"sentinel",{}),"sentinel"in a}catch(c){}}Function.prototype.bind||(Function.prototype.bind=function(a){var c=this;if("function"!=typeof c)throw new TypeError("Function.prototype.bind called on incompatible "+c);var b=q.call(arguments,1),d=function(){if(this instanceof d){var e=function(){};e.prototype=c.prototype;var e=new e,g=c.apply(e,b.concat(q.call(arguments)));
return Object(g)===g?g:e}return c.apply(a,b.concat(q.call(arguments)))};return d});var l=Function.prototype.call,f=Object.prototype,q=Array.prototype.slice,m=l.bind(f.toString),h=l.bind(f.hasOwnProperty),u,v,r,s,p;if(p=h(f,"__defineGetter__"))u=l.bind(f.__defineGetter__),v=l.bind(f.__defineSetter__),r=l.bind(f.__lookupGetter__),s=l.bind(f.__lookupSetter__);Array.isArray||(Array.isArray=function(a){return m(a)=="[object Array]"});Array.prototype.forEach||(Array.prototype.forEach=function(a,c){var b=
i(this),d=-1,e=b.length>>>0;if(m(a)!="[object Function]")throw new TypeError;for(;++d<e;)d in b&&a.call(c,b[d],d,b)});Array.prototype.map||(Array.prototype.map=function(a,c){var b=i(this),d=b.length>>>0,e=Array(d);if(m(a)!="[object Function]")throw new TypeError(a+" is not a function");for(var g=0;g<d;g++)g in b&&(e[g]=a.call(c,b[g],g,b));return e});Array.prototype.filter||(Array.prototype.filter=function(a,c){var b=i(this),d=b.length>>>0,e=[],g;if(m(a)!="[object Function]")throw new TypeError(a+
" is not a function");for(var o=0;o<d;o++)if(o in b){g=b[o];a.call(c,g,o,b)&&e.push(g)}return e});Array.prototype.every||(Array.prototype.every=function(a,c){var b=i(this),d=b.length>>>0;if(m(a)!="[object Function]")throw new TypeError(a+" is not a function");for(var e=0;e<d;e++)if(e in b&&!a.call(c,b[e],e,b))return false;return true});Array.prototype.some||(Array.prototype.some=function(a,c){var b=i(this),d=b.length>>>0;if(m(a)!="[object Function]")throw new TypeError(a+" is not a function");for(var e=
0;e<d;e++)if(e in b&&a.call(c,b[e],e,b))return true;return false});Array.prototype.reduce||(Array.prototype.reduce=function(a){var c=i(this),b=c.length>>>0;if(m(a)!="[object Function]")throw new TypeError(a+" is not a function");if(!b&&arguments.length==1)throw new TypeError("reduce of empty array with no initial value");var d=0,e;if(arguments.length>=2)e=arguments[1];else{do{if(d in c){e=c[d++];break}if(++d>=b)throw new TypeError("reduce of empty array with no initial value");}while(1)}for(;d<b;d++)d in
c&&(e=a.call(void 0,e,c[d],d,c));return e});Array.prototype.reduceRight||(Array.prototype.reduceRight=function(a){var c=i(this),b=c.length>>>0;if(m(a)!="[object Function]")throw new TypeError(a+" is not a function");if(!b&&arguments.length==1)throw new TypeError("reduceRight of empty array with no initial value");var d,b=b-1;if(arguments.length>=2)d=arguments[1];else{do{if(b in c){d=c[b--];break}if(--b<0)throw new TypeError("reduceRight of empty array with no initial value");}while(1)}do b in this&&
(d=a.call(void 0,d,c[b],b,c));while(b--);return d});Array.prototype.indexOf||(Array.prototype.indexOf=function(a){var c=i(this),b=c.length>>>0;if(!b)return-1;var d=0;arguments.length>1&&(d=w(arguments[1]));for(d=d>=0?d:Math.max(0,b+d);d<b;d++)if(d in c&&c[d]===a)return d;return-1});Array.prototype.lastIndexOf||(Array.prototype.lastIndexOf=function(a){var c=i(this),b=c.length>>>0;if(!b)return-1;var d=b-1;arguments.length>1&&(d=Math.min(d,w(arguments[1])));for(d=d>=0?d:b-Math.abs(d);d>=0;d--)if(d in
c&&a===c[d])return d;return-1});Object.getPrototypeOf||(Object.getPrototypeOf=function(a){return a.__proto__||(a.constructor?a.constructor.prototype:f)});Object.getOwnPropertyDescriptor||(Object.getOwnPropertyDescriptor=function(a,c){if(typeof a!="object"&&typeof a!="function"||a===null)throw new TypeError("Object.getOwnPropertyDescriptor called on a non-object: "+a);if(h(a,c)){var b={enumerable:true,configurable:true};if(p){var d=a.__proto__;a.__proto__=f;var e=r(a,c),g=s(a,c);a.__proto__=d;if(e||
g){if(e)b.get=e;if(g)b.set=g;return b}}b.value=a[c];return b}});Object.getOwnPropertyNames||(Object.getOwnPropertyNames=function(a){return Object.keys(a)});Object.create||(Object.create=function(a,c){var b;if(a===null)b={__proto__:null};else{if(typeof a!="object")throw new TypeError("typeof prototype["+typeof a+"] != 'object'");b=function(){};b.prototype=a;b=new b;b.__proto__=a}c!==void 0&&Object.defineProperties(b,c);return b});if(Object.defineProperty){var l=n({}),z="undefined"==typeof document||
n(document.createElement("div"));if(!l||!z)var t=Object.defineProperty}if(!Object.defineProperty||t)Object.defineProperty=function(a,c,b){if(typeof a!="object"&&typeof a!="function"||a===null)throw new TypeError("Object.defineProperty called on non-object: "+a);if(typeof b!="object"&&typeof b!="function"||b===null)throw new TypeError("Property description must be an object: "+b);if(t)try{return t.call(Object,a,c,b)}catch(d){}if(h(b,"value"))if(p&&(r(a,c)||s(a,c))){var e=a.__proto__;a.__proto__=f;
delete a[c];a[c]=b.value;a.__proto__=e}else a[c]=b.value;else{if(!p)throw new TypeError("getters & setters can not be defined on this javascript engine");h(b,"get")&&u(a,c,b.get);h(b,"set")&&v(a,c,b.set)}return a};Object.defineProperties||(Object.defineProperties=function(a,c){for(var b in c)h(c,b)&&b!="__proto__"&&Object.defineProperty(a,b,c[b]);return a});Object.seal||(Object.seal=function(a){return a});Object.freeze||(Object.freeze=function(a){return a});try{Object.freeze(function(){})}catch(E){Object.freeze=
function(a){return function(c){return typeof c=="function"?c:a(c)}}(Object.freeze)}Object.preventExtensions||(Object.preventExtensions=function(a){return a});Object.isSealed||(Object.isSealed=function(){return false});Object.isFrozen||(Object.isFrozen=function(){return false});Object.isExtensible||(Object.isExtensible=function(a){if(Object(a)!==a)throw new TypeError;for(var c="";h(a,c);)c=c+"?";a[c]=true;var b=h(a,c);delete a[c];return b});if(!Object.keys){var x=!0,y="toString toLocaleString valueOf hasOwnProperty isPrototypeOf propertyIsEnumerable constructor".split(" "),
A=y.length,j;for(j in{toString:null})x=!1;Object.keys=function(a){if(typeof a!="object"&&typeof a!="function"||a===null)throw new TypeError("Object.keys called on a non-object");var c=[],b;for(b in a)h(a,b)&&c.push(b);if(x)for(b=0;b<A;b++){var d=y[b];h(a,d)&&c.push(d)}return c}}if(!Date.prototype.toISOString||-1===(new Date(-621987552E5)).toISOString().indexOf("-000001"))Date.prototype.toISOString=function(){var a,c,b,d;if(!isFinite(this))throw new RangeError("Date.prototype.toISOString called on non-finite value.");
a=[this.getUTCMonth()+1,this.getUTCDate(),this.getUTCHours(),this.getUTCMinutes(),this.getUTCSeconds()];d=this.getUTCFullYear();d=(d<0?"-":d>9999?"+":"")+("00000"+Math.abs(d)).slice(0<=d&&d<=9999?-4:-6);for(c=a.length;c--;){b=a[c];b<10&&(a[c]="0"+b)}return d+"-"+a.slice(0,2).join("-")+"T"+a.slice(2).join(":")+"."+("000"+this.getUTCMilliseconds()).slice(-3)+"Z"};Date.now||(Date.now=function(){return(new Date).getTime()});Date.prototype.toJSON||(Date.prototype.toJSON=function(){if(typeof this.toISOString!=
"function")throw new TypeError("toISOString property is not callable");return this.toISOString()});if(!Date.parse||864E13!==Date.parse("+275760-09-13T00:00:00.000Z"))Date=function(a){var c=function g(b,c,d,f,h,i,j){var k=arguments.length;if(this instanceof a){k=k==1&&""+b===b?new a(g.parse(b)):k>=7?new a(b,c,d,f,h,i,j):k>=6?new a(b,c,d,f,h,i):k>=5?new a(b,c,d,f,h):k>=4?new a(b,c,d,f):k>=3?new a(b,c,d):k>=2?new a(b,c):k>=1?new a(b):new a;k.constructor=g;return k}return a.apply(this,arguments)},b=RegExp("^(\\d{4}|[+-]\\d{6})(?:-(\\d{2})(?:-(\\d{2})(?:T(\\d{2}):(\\d{2})(?::(\\d{2})(?:\\.(\\d{3}))?)?(?:Z|(?:([-+])(\\d{2}):(\\d{2})))?)?)?)?$"),
d;for(d in a)c[d]=a[d];c.now=a.now;c.UTC=a.UTC;c.prototype=a.prototype;c.prototype.constructor=c;c.parse=function(c){var d=b.exec(c);if(d){d.shift();for(var f=1;f<7;f++){d[f]=+(d[f]||(f<3?1:0));f==1&&d[f]--}var h=+d.pop(),i=+d.pop(),j=d.pop(),f=0;if(j){if(i>23||h>59)return NaN;f=(i*60+h)*6E4*(j=="+"?-1:1)}h=+d[0];if(0<=h&&h<=99){d[0]=h+400;return a.UTC.apply(this,d)+f-126227808E5}return a.UTC.apply(this,d)+f}return a.parse.apply(this,arguments)};return c}(Date);j="\t\n\x0B\u000c\r \u00a0\u1680\u180e\u2000\u2001\u2002\u2003\u2004\u2005\u2006\u2007\u2008\u2009\u200a\u202f\u205f\u3000\u2028\u2029\ufeff";
if(!String.prototype.trim||j.trim()){j="["+j+"]";var B=RegExp("^"+j+j+"*"),C=RegExp(j+j+"*$");String.prototype.trim=function(){if(this===void 0||this===null)throw new TypeError("can't convert "+this+" to object");return(""+this).replace(B,"").replace(C,"")}}var w=function(a){a=+a;a!==a?a=0:a!==0&&(a!==1/0&&a!==-(1/0))&&(a=(a>0||-1)*Math.floor(Math.abs(a)));return a},D="a"!="a"[0],i=function(a){if(a==null)throw new TypeError("can't convert "+a+" to object");return D&&typeof a=="string"&&a?a.split(""):
Object(a)}});