require 'net/http'
require 'uri'

require 'win32ole'
require 'Win32API'

require 'iconv'


require 'zlib'
require 'stringio'



@cpUTFTo866 = Iconv.new("cp866", "utf-8")
@cp1251To866 = Iconv.new("cp866","windows-1251")
#@cpUTFTo1251 = Iconv.new("windows-1251","utf-8")
#@cp866To1251 = Iconv.new("windows-1251","cp866")

ApiGetConsoleWindow = Win32API.new("kernel32" , "GetConsoleWindow" , ['V'] , 'L')
ConsoleWindowHWND = ApiGetConsoleWindow.call()
ApiSetForegroundWindow = Win32API.new( "user32" , "SetForegroundWindow" , ['L'] , 'i' )


CurrentDirName = File.expand_path(File.dirname(__FILE__))



LogFileName = (CurrentDirName+"/#{Time.now.strftime("%Y%m%d")}.log").to_s 	









	
def _puts string
	log_file = File.open( LogFileName, "a")
	log_file.syswrite("#{Time.now.strftime("%Н:%M:%S")} - #{string}\n")
	log_file.close
	begin
		puts @cpUTFTo866.iconv(string.to_s)
	rescue
		begin
			puts @cp1251To866.iconv(string.to_s)
		rescue
			puts string.to_s
		end
	end
end
def _http_get(get_req, timeout=3)
	_puts get_req
	sleep timeout
	begin
		return @http.get(get_req, @userHeader)
	rescue Exception => e
		_puts "------------"
		_puts e.message
		_puts "------------"
		retry if (e.message=="execution expired")
	end
end

def _http_post(link, data)
	begin
	  return @http.post(link, data, @userHeader)
	rescue Exception => e
		_puts "------------"
		_puts e.message
		_puts "------------"
		retry if (e.message=="execution expired")
	end
end

def _get(get_req, timeout=3)
	resp = _http_get(get_req, timeout)
	#i=Zlib::GzipReader.new(StringIO.new(data0))
	begin
		encoding=resp.response['content-encoding']
	rescue
		return resp.body
	end
	
	case encoding
		when 'gzip'
			content=(Zlib::GzipReader.new(StringIO.new(resp.body))).read
		when 'deflate'
			content=(Zlib::Inflate.new).inflate(resp.body)
		else
			return resp.body
	end
end


def printResponse(response)
	_puts 'Code = ' + response.code
	_puts 'Message = ' + response.message
	response.each {|key, val| _puts key + " : " + val }
end


def vkLogin(url, login, password)

	
	###
	### System logon
	###
	@userHeader = {
	'Host' => 'login.vk.com',
	'User-Agent' => 'Mozilla/5.0 (Windows NT 6.1; WOW64; rv:9.0.1) Gecko/20100101 Firefox/9.0.1',
	'Accept' => 'text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8',
	'Accept-Language' => 'ru-ru,ru;q=0.8,en-us;q=0.5,en;q=0.3',
	#'Accept-Encoding' => 'gzip,deflate',
	'Accept-Charset' => 'windows-1251,utf-8;q=0.7,*;q=0.7',
	'Referer' => 'http://vk.com',
	'Cookie' => 'remixlang=0; remixchk=5; remixflash=10.3.183; remixdt=0',
	'Connection' => 'keep-alive',
	'DNT' => '1'
	}
	
	req = "act=login&q=1&al_frame=1&expire=1&captcha_sid=&captcha_key=&from_host=vk.com&from_protocol=http&email=#{login}&pass=#{password}"
	resp = _http_post(url.path, req)


	# Answer example
	# HTTP/1.1 302 Found
	# Server: nginx/1.0.11
	# Date: Mon, 06 Feb 2012 21:06:57 GMT
	# Content-Type: text/html; charset=windows-1251
	# Connection: keep-alive
	# X-Powered-By: PHP/5.2.6-1+lenny4
	# Set-Cookie: remixlang=0; expires=Wed, 30-Jan-2013 09:55:16 GMT; path=/; domain=.vk.com
	# remixchk=5; expires=Wed, 06-Feb-2013 19:14:22 GMT; path=/; domain=.vk.com
	# s=1; expires=Mon, 11-Feb-2013 04:11:16 GMT; path=/; domain=login.vk.com
	# l=74938; expires=Wed, 30-Jan-2013 17:16:44 GMT; path=/; domain=login.vk.com
	# p=49ef7f2b122c297a6ab801b61e745d6aa10b; expires=Fri, 15-Feb-2013 18:07:14 GMT; path=/; domain=login.vk.com
	# Pragma: no-cache
	# Cache-Control: no-store
	# P3P: CP="IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT"
	# Location: http://vk.com/login.php?act=slogin&al_frame=1&hash=bd7aed27961c325b407332b5855fa1c1&s=1
	# Content-Encoding: gzip
	# Vary: Accept-Encoding
	# Content-Length: 26
	
		
	location=resp.response['location']
	

	#---------------
	# Requesting remixsid
	@userHeader['Host']='vk.com'

	resp = _http_get(location)
	

	# Answer example	
	#HTTP/1.1 200 OK
	#Server: nginx/1.0.11
	#Date: Mon, 06 Feb 2012 21:06:57 GMT
	#Content-Type: text/html; charset=windows-1251
	#Connection: keep-alive
	#X-Powered-By: PHP/5.2.6-1+lenny9
	#Pragma: no-cache
	#Cache-Control: no-store
	#P3P: CP="IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT"
	#Set-Cookie: remixreg_sid=deleted; expires=Sun, 06-Feb-2011 21:06:56 GMT; path=/; domain=.vk.com
	#remixrec_sid=deleted; expires=Sun, 06-Feb-2011 21:06:56 GMT; path=/; domain=.vk.com
	#remixapi_sid=deleted; expires=Sun, 06-Feb-2011 21:06:56 GMT; path=/; domain=.vk.com
	#remixsid=104e5e6d451819d29bd8295b5266918fd966b458247433ea5193c6359def; expires=Sat, 09-Feb-2013 00:38:09 GMT; path=/; domain=.vk.com
	#Content-Encoding: gzip
	#Vary: Accept-Encoding
	#Content-Length: 443
		
	
	remixsid = resp.response['set-cookie'].scan(/remixsid=([\d|a-f]*);/).to_s

	@userHeader.delete('Referer')
	@userHeader['Cookie'] +="; remixsid=#{remixsid}; remixreg_sid=; remixrec_sid=; remixfeed=*.*.*.*.*.ph%2Cvd%2Cnt%2Ctp%2Cgr%2Cev%2Cpr.*"
end
########################################################
########################################################

@peopleFromCity = Hash.new # массив id, href юзеров из данного города


@stat = Array.new(7){Array.new}
@stat[0][0]="Всего\t"
@stat[0][1]=0
@stat[1][0]="Приглашение выслано"
@stat[1][1]=0
@stat[2][0]="Уже в группе (уже идут)"
@stat[2][1]=0
@stat[3][0]="Запретили приглашать себя на встречи"
@stat[3][1]=0
@stat[4][0]="Приглашение уже высылалось"
@stat[4][1]=0
@stat[5][0]="Неприглашенных (не найдено или отказались)"
@stat[5][1]=0
@stat[6][0]="Неизвестная ошибка"
@stat[6][1]=0

def printStat
	
	report_string="\n\t Отчет по городам #{@city_list}:\n"
	for i in 0...7 do
		report_string+="\t#{@stat[i][1]}\t- #{@stat[i][0]}\n"
	end
	total_complete = 0
	for i in 1...7 do
		total_complete+=@stat[i][1]
	end
	report_string+="\t-----------------\n"
	report_string+="\t#{total_complete}\t- Всего обработано\n"
	_puts report_string
end

 
def makeHtmlCaptcha (ie, sid)
	resp  = _http_get("/captcha.php?sid=#{sid}&s=1")
	captcha_name = "captcha#{Time.now.strftime("%Y%m%d_%H%M%S")}"
	
	#open("#{captchaName}.bmp", "wb") { |file| file.write(resp.body) }
	captcha_file_name="#{captcha_name}.jpg"
	File.open(captcha_file_name, "wb") { |file| file.write(resp.body) }
	
	ie.Visible = true
	ie.Silent = true
	ie.MenuBar = false
	ie.ToolBar = false
	ie.Width = 200
	ie.Height = 150
	ie.Navigate("file:////#{CurrentDirName}/#{captcha_name}.jpg")
	#sleep(0.1) until ie.ReadyState == 4
	ApiSetForegroundWindow.call( ConsoleWindowHWND ).to_s
	
	captcha_file_name
end

def inviteCycle(arr_id_hash, group_id)
	captcha_sid = String.new
	cap_key = String.new
	people_from_city_counter=0
	@peopleFromCity.each_key do |id|
		people_from_city_counter+=1 if (captcha_sid.empty?)
		_puts "peopleFromCityCounter=#{people_from_city_counter}"
		
		if not arr_id_hash.has_key?(id)
			@stat[5][1]+=1
			next
		end
		
		data = "act=a_invite&al=1&gid=#{group_id}&hash=#{arr_id_hash.fetch(id)}&mid=#{id}"
		if (not captcha_sid.empty?)
			data +="&captcha_key=#{cap_key}&captcha_sid=#{captcha_sid}"
			captcha_sid.clear
		end
		_puts data
		
		resp, data0 = _http_post("/al_page.php", data)
		#_puts "data0='#{data0}'"
		
		data=data0.scan(/<!--\d*<!><!>\d*<!>\d*<!>\d*<!>(.*)/).to_s
		int_part=data.scan(/<!int>(\d)<!>/).to_s
		if int_part.empty?
			txt_part=data.scan(/(.*)<!>/).to_s
			if (not (captcha_sid=txt_part.scan(/\d{2,}/)).empty?)
				begin
					ie = WIN32OLE.new('InternetExplorer.Application')
				rescue
					_puts "Error #{$!}"
					`taskkill /im iexplore.exe`
					sleep 5
					retry
				end
				captcha_file_name=makeHtmlCaptcha(ie, captcha_sid)
				print " CAPCHA!!!> "; STDOUT.flush; cap_key = gets.chomp
				begin
					`taskkill /im iexplore.exe`
				rescue
					_puts "Error #{$!}"
				end
				# удаление файла с капчей
				File.delete(captcha_file_name)
				# повторная обработка юзера
				redo
				
			# Неизвестная ошибка
			elsif (txt_part.to_s.size==18)
				@stat[6][1]+=1
			else
				_puts "ERR 01"
				_puts "data0='#{data0}'"
				_puts "data='#{data}'"
				_puts "intPart='#{int_part}' txtPart='#{txt_part}' txtPart.size=#{txt_part.to_s.size}"
			end
		else
			txt_part=data.scan(/<!>(.*)/).to_s
			# Пользователь уже в группе.
			if ((int_part == "0") && (txt_part.to_s.size==26))
				@stat[2][1]+=1
			# Пользователь запретил приглашать себя на встречи.
			elsif ((int_part == "0") &&(txt_part.to_s.size==49))
				@stat[3][1]+=1
			# Приглашение уже высылалось.
			elsif ((int_part == "0") &&(txt_part.to_s.size==27))
				@stat[4][1]+=1
			# Приглашение выслано.
			elsif ((int_part == "1") &&(txt_part.to_s.size==20))
				@stat[1][1]+=1
			else
				_puts "ERR 02"
				_puts "data0='#{data0}'"
				_puts "data='#{data}'"
				_puts "intPart='#{int_part}' txtPart='#{txt_part}' txtPart.size=#{txt_part.to_s.size}"
			end				
		end
		
		printStat
	end	
end

def inviteFriendsToEvent(group_id, user_id)
	
	offset = 0
	arr_id_hash = Hash.new
	# получаем hash-коды всех пользователей
	while true
		if (user_id == 0)
			# group members
			get_req = "/friends?act=get_section_friends&al=1&gid=#{group_id}&offset=#{offset}&section=members&sugg_rev=0"
		else
			# friends
			get_req = "/friends?act=load_friends_silent&al=1&gid=#{group_id}&id=#{user_id}"
		end

		data0 = _get(get_req)
			
		# вырезаем всех участников
		data = data0.scan(/\{\s*"members"\s*:\s*\[[ ,0-9a-zA-Z]*\[\s*('.*')\s*\][ ,0-9a-zA-Z]*\]\s*\}/).to_s
		if data.empty?
			_puts data0
			_puts "data.empty data0.scan"
			break
		end

		# разбиваем на строки
		data.gsub!(/'\s*\]\s*[ ,0-9a-zA-Z]*\s*\[\s*'/,"\'\n\'")
		if data.empty? 
			_puts data0
			_puts "data.empty data.gsub"
			break
		end
		# формат участников: 
		#'16795',		'http://cs5527.userapi.com/u16795/d_ef7c524e.jpg','/plotya',	'2',	'0',	'asdfasdf',	'0',	'55',	'10',	'0', '0',	'41deb00d7d428f79d0'

		# вырезаем id и hash
		
		line_count=0
		data.each_line do |subline|
			line_count +=1
			sub_arr=subline.split(/'\s*,\s*'/)
			begin
				hash_val=sub_arr[11].gsub("'","").chomp
				raise "User hash is null" if (hash_val=="0" or hash_val==nil)
				arr_id_hash[sub_arr[0].gsub("'","").chomp] = hash_val
			rescue
				_puts "Error #{$!}"
				_puts "\n\n------------data0------------"
				_puts data0
				_puts "------------data------------"
				_puts data
			end
		end
		
		#общее количество ссылок на аватарки
		http_count = data0.scan(/'http:\/\/\w*.vk.me[\/,\-,\w]*.jpg'/).length
		http_count += data0.scan(/'\/images\/deactivated_100.gif'/).length
		http_count += data0.scan(/'\/images\/camera_b.gif'/).length
		if (http_count != line_count)
			_puts data0
			_puts "httpCount = #{http_count}"
			_puts "lineCount = #{line_count}"
			_puts "httpCount != lineCount"
		end
		
		offset += line_count
		_puts "arrIdHash.length=#{arr_id_hash.length}"
		
	#	i = 1
	#	arrIdHash.each {|id,key| _puts("#{i}. #{id},#{key}"); i=i+1;}
	#	exit
	end
	
	
	inviteCycle(arr_id_hash, group_id)
end


def getAllUsersInCity(group_id, country_id, city_id)
	result = Array.new
	#@peopleFromCity.clear
	offset=0
	for sort_id in 0..1 do
		for status_id in 0..7 do
			for sex_id in 0..2 do
				offset=0
				begin
					get_str = "/al_search.php?al=1"
					get_str += "&c[city]=#{city_id}" if city_id.to_i>0
					get_str += "&c[country]=#{country_id}" if country_id.to_i>0
					get_str += "&c[group]=#{group_id}"
					get_str += "&c[name]=1&c[section]=people"
					get_str += "&c[sex]=#{sex_id}" if sex_id>0
					get_str += "&c[sort]=#{sort_id}" if sort_id>0
					get_str += "&c[status]=#{status_id}" if status_id>0
					get_str += "&offset=#{offset}" if offset>0
					
					data = _get(get_str)
					
					hasmore = data.scan(/<!json>.*"has_more"\s*:\s*(\w*).*<!>/)
					offset = data.scan(/<!json>.*"offset"\s*:\s*(\d*).*<!>/)[0][0].to_i
					result = data.scan(/<a href="(\/[\w,\.]*)"/)
					result.uniq!
					
					result.each do |href| 
						next if @peopleFromCity.has_value?(href)
						user_id=getUserIdbyHref(href)
						if (user_id != 0)
							@peopleFromCity[user_id]=href
						end
					end
										
					_puts "peopleFromCity.size=#{@peopleFromCity.size} hasmore=#{hasmore} offset=#{offset}"
						
				end while hasmore.to_s=="true"
				# прерываем цикл, если не был достигнут предел отсутпа в 1000 на первой итерации
				# на 1000 у контакта отсечка
				break if ((sort_id==0) && (status_id==0) && (sex_id==0) && (offset<1000))
			end #sex
			break if ((sort_id==0) && (status_id==0)  && (offset<1000))
		end #status
		break if ((sort_id==0) && (offset<1000))
	end #sort
	@stat[0][1]=@peopleFromCity.size
end

########################################################
########################################################

def get_city_id_by_name(country_id)
	# функция получения id города по имени

	get_req="/select_ajax.php?act=a_get_cities&country=#{country_id}&str=#{@cityName}"
	# преобразуем в строку вида
	# 'http://vk.com/select_ajax.php?act=a_get_cities&country=1&str=%D0%B4%D0%BC%D0%B8%D1%82%D1%80%D0%BE%D0%B2'
	get_req = URI.escape(get_req)

	data0 = _get(get_req,2)
	if data0.empty?
		_puts data0
		_puts "get_city_id_by_name data0.empty"
		return
	end

	# вырезаем самое первое предложение
	data = data0.scan(/^\[{2,}\'([^\]]*)\'\]/).to_s
	if data.empty?
		_puts data0
		_puts "get_city_id_by_name data.empty"
		return
	end

	# разбиваем на массив значений
	sub_arr=data.split(/'?\s*,\s*'?/)
	_puts "Будем приглашать в город:"
	sub_arr.each{|x| _puts x}

	sub_arr[0]
end

########################################################
########################################################


def getUserIdbyHref(href)
	data0 = _get("http://m.vk.com"+href.to_s,2)
	if data0.empty?
		_puts data0
		_puts "getUserIdbyHref data0.empty"
		return 0
	end
	
	# вырезаем самое первое предложение
	# data = data0.scan(/Profile\.init\(\{"user_id":(\d*),/).to_s
	 data = data0.scan(/<a href="\/friends\?id=(\d*)" class="pm_item">/).to_s
	if data.empty?
		_puts data0
		_puts "getUserIdbyHref data.empty"
		return 0
	end
	
	_puts "data=#{data}"
	
	data
end


########################################################
########################################################


url = URI.parse('http://vk.com/')
@http = Net::HTTP.new(url.host, url.port)
@http.open_timeout = 5
@http.read_timeout = 5

user_id = 0

# id 	Страна
#-------------
# 1 	Россия
# 2 	Украина
# 3		Беларусь
# 4		Казахстан
# 15    Молдова
country_id = "1"

#@city_list=["Санкт-Петербург"]
#@city_list=["Москва", "Балашиха", "Химки", "Подольск", "Королев", "Мытищи", "Люберцы", "Коломна", "Одинцово", "Долгопрудный", "Реутов", "Чехов"]
@city_list = ["Чебоксары"]
#city_id = get_city_id_by_name(country_id)


#id группы-организатора
#nedra 347224
#7rasa 6206
#fahrtdinov 371546
#FERAMONZ 15681844
#Берлога 478898
group_id=6206

#id события
event_id=63653298

login = "..."
password = "..."



vkLogin(url, login, password)

@peopleFromCity.clear

@city_list.each do |@cityName|
	city_id = 0;
	city_id = get_city_id_by_name(country_id)
	next if city_id==0
	getAllUsersInCity(group_id, country_id, city_id)
end

inviteFriendsToEvent(event_id, user_id)
printStat



exit 