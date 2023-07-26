let { createApp } = Vue

createApp({
    data() {
        return {
            list: [],
            active: false,
            tableHeight: 500
        }
    },
    mounted() {
    if (document.body.offsetWidth <= 768) {
        this.tableHeight = 300
    }
    },
    methods: {
    handleChange(file) {
        let that = this
        if (! /.har/.test(file.name)) {
            ElementPlus.ElMessage.error('请上传Har文件')
            return false;
        }
        ElementPlus.ElMessage.warning('解析中。。。')
        var reader = new FileReader();
        reader.onload = (e) => {
            // console.log(e.target.result)
            let resultObject = JSON.parse(reader.result)
            let entries = resultObject.log.entries
            let list = []
            entries.forEach(e => {
                if (/image/.test(e.response.content.mimeType) && ! /favicon.ico|hm.gif|google-analytics|cnzz/.test(e.request.url)) {
                    let url = e.request.url
                    let item = {
                        url: url,
                        name: url.split('/')[url.split('/').length - 1],
                        size: e.response.content.size,
                        base64: 'data:image/png;base64,' + e.response.content.text
                    }
                    list.push(item)
                }
            });
            // console.log(list)
            ElementPlus.ElMessage.success('解析完毕')
            that.list = list
            if (list.length > 0) {
                that.active = true
            }
        };
        reader.readAsText(file.raw);
    },
    handleRemove() {
        this.list = []
        this.active = false
    },
    filesize(fileByte = 0) {
        var fileByte = Number(fileByte)
        var fileSizeMsg = "";
        if (fileByte < 1048576) fileSizeMsg = (fileByte / 1024).toFixed(2) + "KB";
        else if (fileByte == 1048576) fileSizeMsg = "1MB";
        else if (fileByte > 1048576 && fileByte < 1073741824) fileSizeMsg = (fileByte / (1024 * 1024)).toFixed(2) + "MB";
        else if (fileByte > 1048576 && fileByte == 1073741824) fileSizeMsg = "1GB";
        else if (fileByte > 1073741824 && fileByte < 1099511627776) fileSizeMsg = (fileByte / (1024 * 1024 * 1024)).toFixed(2) + "GB";
        else fileSizeMsg = "文件大小超过1TB";
        return fileSizeMsg;
    },
    getImageBase64(image) {
        let canvas = document.createElement('canvas')
        canvas.width = image.width
        canvas.height = image.height
        let ctx = canvas.getContext('2d')
        ctx.drawImage(image, 0, 0, image.width, image.height)
        // 获取图片后缀名
        let extension = image.src
            .substring(image.src.lastIndexOf('.') + 1)
            .toLowerCase()
        // 某些图片 url 可能没有后缀名，默认是 png
        return canvas.toDataURL('image/' + (extension ? extension : 'png'), 1)
    },
    downloadSingle(url, downloadName) {
        let that = this
        let link = document.createElement('a')
        link.setAttribute('download', downloadName)
        let image = new Image()
        let time = new Date().getTime()
        // 添加时间戳，防止浏览器缓存图片
        image.src = /\?/.test(url) ? url + '&timestamp=' + time : url + '?timestamp=' + time
        // 设置 crossOrigin 属性，解决图片跨域报错
        image.setAttribute('crossOrigin', 'Anonymous')
        image.onload = () => {
            link.href = that.getImageBase64(image)
            link.click()
        }
    },
    downloadZip() {
        let that = this
        ElementPlus.ElMessageBox.prompt('请输入保存文件名', '提示', {
            confirmButtonText: '确认打包',
            cancelButtonText: '取消',
            inputValue: '原神网页小活动资源包',
            inputPattern: /^[\u4E00-\u9FA5A-Za-z0-9_]+$/,
            inputErrorMessage: '只允许中文、英文、数字和下划线',
        }).then(({ value }) => {
            let zipName = value
            let zip = new JSZip()
            let fileFolder = zip.folder(zipName) // 创建 zipName 文件夹
            let fileList = []
            ElementPlus.ElMessage.warning('正在打包中。。。')
            this.list.forEach(e => {
                let name = e.name
                let image = new Image()
                image.setAttribute('crossOrigin', 'Anonymous') // 设置 crossOrigin 属性，解决图片跨域报错
                // 添加时间戳，防止浏览器缓存图片
                let time = new Date().getTime()
                image.src = /\?/.test(e.url) ? e.url + '&timestamp=' + time : e.url + '?timestamp=' + time
                image.onload = async() => {
                    let url = await that.getImageBase64(image)
                    fileList.push({
                        name: name,
                        img: url.substring(22) // 截取 data:image/png;base64, 后的数据
                    })
                    if (fileList.length === that.list.length) {
                        if (fileList.length) {
                            for (let k = 0; k < fileList.length; k++) {
                                // 往文件夹中，添加每张图片数据
                                fileFolder.file(fileList[k].name + '.png', fileList[k].img, {
                                    base64: true
                                })
                            }
                            zip.generateAsync({ type: 'blob' }).then(content => {
                                ElementPlus.ElMessage.success('打包完毕')
                                saveAs(content, zipName + '.zip')
                            })
                        }
                    }
                }
            });
        })
    }
    }
}).use(ElementPlus).mount('#app')
