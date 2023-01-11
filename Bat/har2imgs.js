const fs = require('fs')
const assert = require('assert')

let input = process.argv[2]
let prefix = input.substr(0, input.lastIndexOf('.'))
let errLog = `err/err-${prefix}.json`
let saveFolder = `${prefix}`

!fs.existsSync(saveFolder) && fs.mkdirSync(saveFolder)
!fs.existsSync('err') && fs.mkdirSync('err')

let err = []
let objArr = JSON.parse(fs.readFileSync(input, 'utf8')).log.entries

objArr.map(o => {
	let url = o.request.url
	let filename = url.substr(url.lastIndexOf('/')+1)
	let base64 = o.response.content.text

	try {
		assert.ok(base64 && base64.length > 0)
		let buffer = new Buffer(base64, 'base64')
		fs.writeFileSync(`./${saveFolder}/${filename}`, buffer)
		console.log(`${filename} extracted`)
	} catch(e) {
		err.push(url)
	}
})

err.map(filename => console.log(`${filename} failed`))
err.length > 0 && fs.writeFileSync(errLog, JSON.stringify(err))

console.log(`提取完成，一共 ${objArr.length} 个，成功 ${objArr.length - err.length} 个，失败 ${err.length} 个`)
err.length > 0 && console.log(`错误记录已写入 ${errLog} `)