// src/server.js
import { createServer, Model } from "miragejs"

export function makeServer({ environment = "development" } = {}) {
  let server = createServer({
    environment,

    models: {
      user: Model,
      purchaseOrder: Model,
      purchaseOrderDetailUnit: Model,
      purcahseOrderOtherUnit: Model,
    },

    seeds(server) {
      server.create("user", { id:"1", name: "Bob" })
      server.create("user", { id:"2", name: "Alice" })

      server.create("purchaseOrder", {id: "1", prNo : "PR-123", prCode : ""});
    },

    routes() {
      this.namespace = "api"
      this.passthrough('https://dev-amn.au.auth0.com/**');
      this.passthrough('http://pslb3.menlhk.go.id/**');
      this.passthrough('https://campustecnologicoalgeciras.es/**');
      this.get("/users", (schema) => {
        return schema.users.all()
      })

      this.get("/purchaserequisition", () => ({
        "pageIndex": 0,
        "pageSize": 5,
        "count": 12,
        "data": [
          {
            id : "1",
            prNo : "PR/123",
            prCode : "PR-C/123",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            image : "",
            otherUnits : [
              {
                id:"1",
                mode : 'MODE 3',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                purchase_order_id : "1"
            }] 
          },
          {
            id : "2",
            prNo : "PR/234",
            prCode : "PR-C/234",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            image : "",
          },
          {
            id : "3",
            prNo : "PR/345",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            image : "",
          },
        ]
      }))

      this.get("/purchaserequisition/:id", () => (
        {
          id : "1",
          prNo : "PR/123",
          prCode : "PR-C/123",
          warehouse : {
            id: "1",
            name : "Sunter 3"
          },
          pdfFile : "",
          location : "Sunter",
          isReceived : true,
          plodDate : "2021-01-18",
          image : "",
          otherUnits : [
            {
              id:"1",
              mode : 'MODE 3',
              katashikiSuffix : 'DU 3',
              tipe : 'TIPE 3',
              hasColor : 'Yes',
              plodDate : '20-11-2021',
              purchase_order_id : "1"
          }] 
        }
      ))

      this.post("/purchaserequisition", () => (
        {
          id : "1",
          prNo : "PR/123",
          prCode : "PR-C/123",
          warehouse : {
            id: "1",
            name : "Sunter 3"
          },
          pdfFile : "",
          location : "Sunter",
          isReceived : true,
          plodDate : "2021-01-18",
          image : "",
          otherUnits : [
            {
              id:"1",
              mode : 'MODE 3',
              katashikiSuffix : 'DU 3',
              tipe : 'TIPE 3',
              hasColor : 'Yes',
              plodDate : '20-11-2021',
              purchase_order_id : "1"
          }] 
        }
      ))

      this.patch("/purchaserequisition/:id", () => (
        {
          id : "1",
          prNo : "PR/123",
          prCode : "PR-C/123",
          warehouse : {
            id: "1",
            name : "Sunter 3"
          },
          pdfFile : "",
          location : "Sunter",
          isReceived : true,
          plodDate : "2021-01-18",
          image : "",
          otherUnits : [
            {
              id:"1",
              mode : 'MODE 3',
              katashikiSuffix : 'DU 3',
              tipe : 'TIPE 3',
              hasColor : 'Yes',
              plodDate : '20-11-2021',
              purchase_order_id : "1"
          }] 
        }
      ))

      this.post("/purchaseorderotherunits/upload", () => (
        [
            {
              id:"1",
              mode : 'MODE 1',
              katashikiSuffix : 'DU 1',
              tipe : 'TIPE 1',
              hasColor : 'Yes',
              plodDate : '20-11-2021',
              isFromUpload: true,
              purchase_order_id : "1"
            },
            {
              id:"2",
              mode : 'MODE 2',
              katashikiSuffix : 'DU 2',
              tipe : 'TIPE 2',
              hasColor : 'Yes',
              plodDate : '20-11-2021',
              isFromUpload: true,
              purchase_order_id : "1"
            },
            {
              id:"3",
              mode : 'MODE 3',
              katashikiSuffix : 'DU 3',
              tipe : 'TIPE 3',
              hasColor : 'Yes',
              plodDate : '20-11-2021',
              isFromUpload: true,
              purchase_order_id : "1"
            }
        ]
      ))

      this.get("/warehouse", () => ({
        "pageIndex": 0,
        "pageSize": 5,
        "count": 12,
        "data": [
          {
            id : "1",
            name : "Warehouse 1",
          },
          {
            id : "2",
            name : "Warehouse 2",
          },
          {
            id : "3",
            name : "Warehouse 3",
          },
        ]
      }))

      this.get("/gudang", () => ({"pageIndex":0,"pageSize":10,"pageCount":2,"dataCount":13,"data":[{"id":1,"name":"Gudang Satu","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":2,"name":"Gudang Dua","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":3,"name":"Gudang Tiga","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":5,"name":"Gudang AHASS","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":6,"name":"Gudang Empat","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":7,"name":"Gudang Lima","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":8,"name":"Gudang Enam","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":9,"name":"Gudang Tujuh","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":10,"name":"Gudang Delapan","uploadValidationStatus":null,"uploadValidationMessage":null},{"id":11,"name":"Gudang Sembilan","uploadValidationStatus":null,"uploadValidationMessage":null}]}))

      this.get("/role/:id", () => ({"id":1,"name":"Administrator","description":"Administrator","roleDetails":[{"id":1,"roleId":1,"functionInfoId":"user_info","functionName":"User Management","allowCreate":true,"allowRead":true,"allowUpdate":true,"allowDelete":true,"showInMenu":true,"allowDownload":true,"allowPrint":true},{"id":2,"roleId":1,"functionInfoId":"function_info","functionName":"Function Management","allowCreate":true,"allowRead":true,"allowUpdate":true,"allowDelete":true,"showInMenu":true,"allowDownload":true,"allowPrint":true},{"id":3,"roleId":1,"functionInfoId":"role","functionName":"Role Management","allowCreate":true,"allowRead":true,"allowUpdate":true,"allowDelete":true,"showInMenu":true,"allowDownload":true,"allowPrint":true},{"id":4,"roleId":1,"functionInfoId":"user_role","functionName":"User Role Management","allowCreate":true,"allowRead":true,"allowUpdate":true,"allowDelete":true,"showInMenu":true,"allowDownload":true,"allowPrint":true},{"id":5,"roleId":1,"functionInfoId":"gudang","functionName":"Gudang","allowCreate":true,"allowRead":true,"allowUpdate":true,"allowDelete":true,"showInMenu":true,"allowDownload":true,"allowPrint":true},{"id":6,"roleId":1,"functionInfoId":"semua","functionName":"Semua","allowCreate":true,"allowRead":true,"allowUpdate":true,"allowDelete":true,"showInMenu":true,"allowDownload":true,"allowPrint":true}]}))

      this.post("/gudang/singlepagepdf", () => (
        { download:'http://pslb3.menlhk.go.id/internal/uploads/pengumuman/1545111808_contoh-pdf.pdf' }
      ))

      this.get("/location", () => ({
        "pageIndex": 0,
        "pageSize": 5,
        "count": 12,
        "data": [
          {
            id : "1",
            name : "Location 1",
          },
          {
            id : "2",
            name : "Location 2",
          },
          {
            id : "3",
            name : "Location 3",
          },
        ]
      }))

      this.get("/katashifi", () => ({
        "pageIndex": 0,
        "pageSize": 5,
        "count": 12,
        "data": [
          {
            id : "1",
            name : "Katashifi 1",
          },
          {
            id : "2",
            name : "Katashifi 2",
          },
          {
            id : "3",
            name : "Katashifi 3",
          },
        ]
      }))

      this.get("/purchaserequisition/ids", () => (
        [1,2,3]
      ))

      this.post("/purchaserequisition/multipagepdf", () => (
        {
          id : '123'
        }
      ))

      this.post("/purchaserequisition/downloaddata", () => (
        {
          id : '123'
        }
      ))

      this.post("/purchaserequisition/singlepagepdf", () => (
        {
          url : 'http://pslb3.menlhk.go.id/internal/uploads/pengumuman/1545111808_contoh-pdf.pdf'
        }
      ))

      this.post("/attachment/upload", () => (
        {
          id : "1",
          originalFilename : "file.pdf",
          savedFileName : "filexxx.pdf",
          fileExtension : "pdf",
          fileSize : "2500",
        }
      ))
      
      this.post("/purchaserequisition/confirmUpload", () => (
        {
          success : true
        }
      ))

      this.post("/purchaserequisition/upload", () => (
        [
          {
            id : "1",
            prNo : "PR/123",
            prCode : "PR-C/123",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            isFromUpload : true,
            plodDate : "2021-01-18",
            image : "",
            otherUnits : [
              {
                id:"1",
                mode : 'MODE 1',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"2",
                mode : 'MODE 2',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"3",
                mode : 'MODE 3',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"4",
                mode : 'MODE 4',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"5",
                mode : 'MODE 5',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"6",
                mode : 'MODE 6',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"7",
                mode : 'MODE 7',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
            ] 
          },
          {
            id : "2",
            prNo : "PR/234",
            prCode : "PR-C/234",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
            otherUnits : [
              {
                id:"11",
                mode : 'MODE 11',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"12",
                mode : 'MODE 12',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"13",
                mode : 'MODE 13',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"14",
                mode : 'MODE 14',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"15",
                mode : 'MODE 15',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"16",
                mode : 'MODE 16',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
              {
                id:"17",
                mode : 'MODE 17',
                katashikiSuffix : 'DU 3',
                tipe : 'TIPE 3',
                hasColor : 'Yes',
                plodDate : '20-11-2021',
                isFromUpload : true,
                purchase_order_id : "1"
            
              },
            ]
          },
          {
            id : "3",
            prNo : "PR/345",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "4",
            prNo : "PR/445",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "5",
            prNo : "PR/545",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "6",
            prNo : "PR/645",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "7",
            prNo : "PR/745",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "8",
            prNo : "PR/845",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "9",
            prNo : "PR/945",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "10",
            prNo : "PR/1045",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "11",
            prNo : "PR/1145",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
          {
            id : "12",
            prNo : "PR/1245",
            prCode : "PR-C/345",
            warehouse : {
              id : "1",
              name : "Warehouse 1",
            },
            pdfFile : "",
            location : {
              id : "1",
              name : "Location 1",
            },
            isReceived : true,
            plodDate : "2021-01-18",
            isFromUpload : true,
            image : "",
          },
        ]
      ))
    },
  })

  return server
}