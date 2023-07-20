var cssdropdown={disappeardelay:250,dropdownindicator:'<img src="/chrometheme/down.gif" border="0" />',enablereveal:[true,5],enableiframeshim:1,dropmenuobj:null,asscmenuitem:null,domsupport:document.all||document.getElementById,standardbody:null,iframeshimadded:false,revealtimers:{},getposOffset:function(what,offsettype){var totaloffset=(offsettype=="left")?what.offsetLeft:what.offsetTop;var parentEl=what.offsetParent;while(parentEl!=null){totaloffset=(offsettype=="left")?totaloffset+parentEl.offsetLeft:totaloffset+parentEl.offsetTop;parentEl=parentEl.offsetParent;}
return totaloffset;},css:function(el,targetclass,action){var needle=new RegExp("(^|\\s+)"+targetclass+"($|\\s+)","ig")
if(action=="check")
return needle.test(el.className)
else if(action=="remove")
el.className=el.className.replace(needle,"")
else if(action=="add"&&!needle.test(el.className))
el.className+=" "+targetclass},showmenu:function(dropmenu,e){if(this.enablereveal[0]){if(!dropmenu._trueheight||dropmenu._trueheight<10)
dropmenu._trueheight=dropmenu.offsetHeight
clearTimeout(this.revealtimers[dropmenu.id])
dropmenu.style.height=dropmenu._curheight=0
dropmenu.style.overflow="hidden"
dropmenu.style.visibility="visible"
this.revealtimers[dropmenu.id]=setInterval(function(){cssdropdown.revealmenu(dropmenu)},10)}
else{dropmenu.style.visibility="visible"}
this.css(this.asscmenuitem,"selected","add")},revealmenu:function(dropmenu,dir){var curH=dropmenu._curheight,maxH=dropmenu._trueheight,steps=this.enablereveal[1]
if(curH<maxH){var newH=Math.min(curH,maxH)
dropmenu.style.height=newH+"px"
dropmenu._curheight=newH+Math.round((maxH-newH)/steps)+1}
else{dropmenu.style.height="auto"
dropmenu.style.overflow="hidden"
clearInterval(this.revealtimers[dropmenu.id])}},clearbrowseredge:function(obj,whichedge){var edgeoffset=0
if(whichedge=="rightedge"){var windowedge=document.all&&!window.opera?this.standardbody.scrollLeft+this.standardbody.clientWidth-15:window.pageXOffset+window.innerWidth-15
var dropmenuW=this.dropmenuobj.offsetWidth
if(windowedge-this.dropmenuobj.x<dropmenuW)
edgeoffset=dropmenuW-obj.offsetWidth}
else{var topedge=document.all&&!window.opera?this.standardbody.scrollTop:window.pageYOffset
var windowedge=document.all&&!window.opera?this.standardbody.scrollTop+this.standardbody.clientHeight-15:window.pageYOffset+window.innerHeight-18
var dropmenuH=this.dropmenuobj._trueheight
if(windowedge-this.dropmenuobj.y<dropmenuH){edgeoffset=dropmenuH+obj.offsetHeight
if((this.dropmenuobj.y-topedge)<dropmenuH)
edgeoffset=this.dropmenuobj.y+obj.offsetHeight-topedge}}
return edgeoffset},dropit:function(obj,e,dropmenuID){if(this.dropmenuobj!=null)
this.hidemenu()
this.clearhidemenu()
this.dropmenuobj=document.getElementById(dropmenuID)
this.asscmenuitem=obj
this.showmenu(this.dropmenuobj,e)
this.dropmenuobj.x=this.getposOffset(obj,"left")
this.dropmenuobj.y=this.getposOffset(obj,"top")
this.dropmenuobj.style.left=this.dropmenuobj.x-this.clearbrowseredge(obj,"rightedge")+"px"
this.dropmenuobj.style.top=this.dropmenuobj.y-this.clearbrowseredge(obj,"bottomedge")+obj.offsetHeight+1+"px"
this.positionshim()},positionshim:function(){if(this.iframeshimadded){if(this.dropmenuobj.style.visibility=="visible"){this.shimobject.style.width=this.dropmenuobj.offsetWidth+"px"
this.shimobject.style.height=this.dropmenuobj._trueheight+"px"
this.shimobject.style.left=parseInt(this.dropmenuobj.style.left)+"px"
this.shimobject.style.top=parseInt(this.dropmenuobj.style.top)+"px"
this.shimobject.style.display="block"}}},hideshim:function(){if(this.iframeshimadded)
this.shimobject.style.display='none'},isContained:function(m,e){var e=window.event||e
var c=e.relatedTarget||((e.type=="mouseover")?e.fromElement:e.toElement)
while(c&&c!=m)try{c=c.parentNode}catch(e){c=m}
if(c==m)
return true
else
return false},dynamichide:function(m,e){if(!this.isContained(m,e)){this.delayhidemenu()}},delayhidemenu:function(){this.delayhide=setTimeout("cssdropdown.hidemenu()",this.disappeardelay)},hidemenu:function(){this.css(this.asscmenuitem,"selected","remove")
this.dropmenuobj.style.visibility='hidden'
this.dropmenuobj.style.left=this.dropmenuobj.style.top="-1000px"
this.hideshim()},clearhidemenu:function(){if(this.delayhide!="undefined")
clearTimeout(this.delayhide)},addEvent:function(target,functionref,tasktype){if(target.addEventListener)
target.addEventListener(tasktype,functionref,false);else if(target.attachEvent)
target.attachEvent('on'+tasktype,function(){return functionref.call(target,window.event)});},startchrome:function(){if(!this.domsupport)
return
this.standardbody=(document.compatMode=="CSS1Compat")?document.documentElement:document.body
for(var ids=0;ids<arguments.length;ids++){var menuitems=document.getElementById(arguments[ids]).getElementsByTagName("a")
for(var i=0;i<menuitems.length;i++){if(menuitems[i].getAttribute("rel")){var relvalue=menuitems[i].getAttribute("rel")
var asscdropdownmenu=document.getElementById(relvalue)
this.addEvent(asscdropdownmenu,function(){cssdropdown.clearhidemenu()},"mouseover")
this.addEvent(asscdropdownmenu,function(e){cssdropdown.dynamichide(this,e)},"mouseout")
this.addEvent(asscdropdownmenu,function(){cssdropdown.delayhidemenu()},"click")
try{menuitems[i].innerHTML=menuitems[i].innerHTML+" "+this.dropdownindicator}catch(e){}
this.addEvent(menuitems[i],function(e){if(!cssdropdown.isContained(this,e)){var evtobj=window.event||e
cssdropdown.dropit(this,evtobj,this.getAttribute("rel"))}},"mouseover")
this.addEvent(menuitems[i],function(e){cssdropdown.dynamichide(this,e)},"mouseout")
this.addEvent(menuitems[i],function(){cssdropdown.delayhidemenu()},"click")}}}
if(this.enableiframeshim&&document.all&&!window.XDomainRequest&&!this.iframeshimadded){document.write('<IFRAME id="iframeshim" src="about:blank" frameBorder="0" scrolling="no" style="left:0; top:0; position:absolute; display:none;z-index:90; background: transparent;"></IFRAME>')
this.shimobject=document.getElementById("iframeshim")
this.shimobject.style.filter='progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)'
this.iframeshimadded=true}}}