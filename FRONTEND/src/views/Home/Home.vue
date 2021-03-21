<template>
  <div>
    <b-row>
        <div class="">News</div>
    </b-row>
    <b-row class="mt-0">
        <b-col no-body lg="4" md="6" sm="12" class="mt-3" v-for="item in news" v-bind:key="item.index">
            <b-card>
                <b-row no-gutters>
                    <b-col cols="3">
                        <b-card-img :src="item.image" alt="Image" ></b-card-img>
                    </b-col>
                    <b-col cols="9">
                        <b-card-body :title="item.title" title-tag="h5" :sub-title="item.publishedDate" sub-title-tag="h6" class="p-0 pl-2">
                            <b-card-text>
                            </b-card-text>
                        </b-card-body>
                    </b-col>
                </b-row>
            </b-card>
        </b-col>
    </b-row>
  </div>
</template>
<style lang="scss" scoped>
    /deep/ h5.card-title {
        margin: 0;
    }

    /deep/ h6.card-subtitle {
        font-size: 0.75rem !important;
        margin-top: 0.5px !important;
    }
    .btn-primary {
        background-color: #029FE2;
        border-color: #029FE2;
    }

    .btn-primary:hover {
        background-color: #029FE2;
        border-color: #029FE2;
    }

    .menu-group {
        width: 190px;
    }

    .menu-icon {
        width: 85px;
        height: 85px;
        /*border: 1px solid #ddd;*/
        background-color: transparent;
        border-radius: 5px;
        display: block;
        margin-left: auto;
        margin-right: auto;
        /*padding: 15px;*/
    }

        .menu-icon i {
            font-size: 45px;
            color: #222;
        }

        .menu-icon-img {
            width: 80px;
        }

        .menu-group-title {
            font-size: 14px;
            font-weight: bold;
            /*color: #565656;*/
            /*word-spacing: 100vw;*/
        }

        .menu-group {
            float: left;
            margin-right: 10px;
        }

        .button-menu-footer, .card-footer {
            padding: 8px;
        }

            .button-menu-footer .button-menu-item {
                padding-top: 5px;
                padding-bottom: 5px;
                text-align: center;
            }

                .button-menu-footer .button-menu-item:hover {
                    background-color: #eee;
                    cursor: pointer;
                }

        .box-news-information {
            /*border: 1px solid #dcdcdc;*/
            padding: 0px;
            right: 0px;
            /*background-color: white;*/
            border-radius: 5px;
            height: 500px;
        }

        #box-news-information {
            overflow-y: scroll;
            overflow-x: hidden;
            height: 484px;
            margin-top: 10px;
            margin-bottom: 10px;
        }

        .badge-info {
            font: bold 11px/1 "Roboto", Arial, sans-serif;
            background-color: #fff;
        }

        .badge-info-news {
            color: #0085b6;
        }

        .badge-information {
            color: #8360a8;
        }

        .news-title {
            margin-bottom: -4px;
            font: 500 14px/1.4em "Roboto", Arial, sans-serif;
            color: #4a4a4a;
        }

        .news-info {
            margin-bottom: 3px;
        }

        .panel-newsinfo {
            border-color: #d2d2d2;
            margin-right: 10px;
            padding-left: 0px;
        }

            .panel-newsinfo > .panel-heading {
                background-color: #fff;
                border-color: #d2d2d2;
            }

            .panel-newsinfo > .panel-body {
                padding: 0px;
            }

        .panel-heading {
            border-bottom: 1px solid #ddd;
        }
        /*
        .col-custom {
            width: 149px;
            padding: 0px 3px 0px 3px;
            display: inline-flex;
            flex-direction: column;
        }*/

        .panel-body-leave {
            background-color: #fff;
        }

        .panel-footer {
            background-color: #fff;
        }

        .own_contain {
            padding-top: 5px;
        }

        .notification-counter {
            border-radius: 16px;
            background-color: #ff6124;
            padding: 3px 5px;
            color: #fff;
            margin-left: -8px;
            font-size: 11px;
            top: 2px;
            position: absolute;
        }

        .btn-default {
            font-size: 11px;
        }

        .panel-body-leave .panel-heading {
            background-color: #f1f4ff;
            color: #545c79;
        }

        .panel-body-reimbursement .panel-heading {
            background-color: #dff0d8;
            color: #526b48;
        }

        .panel-body-overtime .panel-heading {
            background-color: #fcf8e3;
            color: #88773a;
        }

        .panel-body-medical .panel-heading {
            background-color: #d8f0dd;
            color: #1b6961;
        }

        .panel-body-attendance .panel-heading {
            background-color: #d8eff0;
            color: #1b6961;
        }

        .panel-body-resignation .panel-heading {
            background-color: #f9eaea;
            color: #9e6b66;
        }

        .menu-group .panel .panel-heading {
            padding: 3px;
        }

        .button-menu-footer .row .col-xs-6 {
            padding-left: 11px;
        }

            .button-menu-footer .row .col-xs-6:not(:first-child) {
                padding-left: 5px;
            }

        .btn-first, .btn-second {
            font-size: 10.5px;
            width: 85px;
        }

        .btn-second {
            padding-left: 5px;
        }

        .fa-envelope {
            font-size: 20px;
        }

        #my-inbox-txt {
            font-size: 14px;
            margin-left: 10px;
        }

        @media (min-width: 768px) {
            .panel-right {
                padding-left: 0px;
            }
        }

        @media (max-width: 410px) {
            .menu-group {
                width: 100%;
            }

            .col-xs-11 {
                width: 89.666667%;
            }

            .hidden-xxs {
                display: none;
            }

            .panel-newsinfo {
                margin-right: 0px;
            }

            .btn-first, .btn-second {
                font-size: 11px;
                width: 100%;
            }
        }

        @media (min-width: 411px) {
            .menu-group {
                width: 180px;
            }

            .btn-create-request {
                padding-left: 4px;
                margin-left: -2px;
            }

            .btn-first, .btn-second {
                font-size: 10px;
                width: 80px;
            }
        }
</style>
<script>
import axios from 'axios';

export default {
    components : {},
    name: 'Home',
    methods : {
      getApplicationDefinitions : function() {
        this.isDataLoading = true;
        /*axios
            .get(process.env.VUE_APP_API_URL + "ApplicationDefinitions",
             {
               params: {
                 name:this.name
               }
             }
            )
            .then(response => { 
              let datas = [];
              if (response.data.length > 0) {
                let index = 0;
                response.data.forEach(element => {
                  index++;
                  let data = {
                    "no" : index,
                    "id" : element.id,
                    "applicationName" : element.applicationName,
                    "createdDate" : element.createdDate,
                  }
                  datas.push(data);
                });
              }
              this.items = datas;
              this.isDataLoading = false;
            })
            .catch(errors => {
                console.log(errors);
                this.isDataLoading = false;
            });*/
      }
    },
    data() {
      return {
        perPage: 14,
        currentPage: 1,
        name:'',
        fields: ["no", "applicationName", "createdDate", "actions"],
        items: [],
        isDataLoading: true,
        news: [
            {
                image : "https://editorial.pxcrush.net/carsales/general/editorial/toyota-yaris-cross-gxl-hybrid-05.jpg?width=1024&height=683",
                title : "Harga tembus Rp 1 miliar, Toyota Yaris Cross meluncur di Singapura",
                publishedDate : "Senin, 18 Januari 2021 / 05:43 WIB"
            },
            {
                image : "https://editorial.pxcrush.net/carsales/general/editorial/toyota-yaris-cross-gxl-hybrid-05.jpg?width=1024&height=683",
                title : "Harga tembus Rp 1 miliar, Toyota Yaris Cross meluncur di Singapura",
                publishedDate : "Senin, 18 Januari 2021 / 05:43 WIB"
            },
            {
                image : "https://editorial.pxcrush.net/carsales/general/editorial/toyota-yaris-cross-gxl-hybrid-05.jpg?width=1024&height=683",
                title : "Harga tembus Rp 1 miliar, Toyota Yaris Cross meluncur di Singapura",
                publishedDate : "Senin, 18 Januari 2021 / 05:43 WIB"
            },
            {
                image : "https://editorial.pxcrush.net/carsales/general/editorial/toyota-yaris-cross-gxl-hybrid-05.jpg?width=1024&height=683",
                title : "Harga tembus Rp 1 miliar, Toyota Yaris Cross meluncur di Singapura",
                publishedDate : "Senin, 18 Januari 2021 / 05:43 WIB"
            },
        ]
      }
    },
    mounted() {
        let breadcrumb = [];
        this.$store.dispatch('setBreadCrumb', breadcrumb);
    },
    computed: {
      rows() {
        return this.items.length
      },
      dataList() 
      {
          if (this.items != undefined) {
              return this.items.slice(((this.currentPage - 1) * this.perPage),((this.currentPage - 1) * this.perPage) + this.perPage);
          }
          return [];
      }
    }
}
</script>