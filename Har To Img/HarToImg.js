const fs = require('fs');
const assert = require('assert');

let input = process.argv[2]; // 从命令行参数获取输入文件名
let prefix = input.substr(0, input.lastIndexOf('.')); // 获取输入文件名的前缀
let errLog = `err/err-${prefix}.json`; // 错误记录文件的路径及名称
let saveFolder = `${prefix}`; // 文件保存目录的名称，与输入文件名前缀相同

!fs.existsSync(saveFolder) && fs.mkdirSync(saveFolder); // 如果保存目录不存在，则创建它
!fs.existsSync('err') && fs.mkdirSync('err'); // 如果错误记录目录不存在，则创建它

let err = []; // 用于存储处理中出现错误的URL列表
let objArr = JSON.parse(fs.readFileSync(input, 'utf8')).log.entries; // 从输入文件中读取JSON对象数组

objArr.map(o => {
	let url = o.request.url; // 获取请求的URL
	let filename = url.substr(url.lastIndexOf('/') + 1); // 从URL中提取文件名
	let base64 = o.response.content.text; // 获取响应内容的Base64编码字符串

	try {
		assert.ok(base64 && base64.length > 0); // 断言确保Base64编码内容存在且长度大于0
		let buffer = new Buffer.from(base64, 'base64'); // 将Base64编码转换为Buffer
		fs.writeFileSync(`./${saveFolder}/${filename}`, buffer); // 将Buffer内容写入文件
		console.log(`${filename} extracted`); // 输出提取成功的信息
	} catch (e) {
		err.push(url); // 将处理失败的URL添加到错误列表中
	}
});

err.map(filename => console.log(`${filename} failed`)); // 输出处理失败的URL列表
if (err.length > 0) {
	fs.writeFileSync(errLog, JSON.stringify(err)); // 将错误列表写入错误记录文件
}

console.log(`提取完成，一共 ${objArr.length} 个，成功 ${objArr.length - err.length} 个，失败 ${err.length} 个`); // 输出处理结果统计
if (err.length > 0) {
	console.log(`错误记录已写入 ${errLog}`); // 如果有错误发生，输出错误记录文件的路径
}
