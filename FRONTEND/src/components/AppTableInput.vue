<template>
    <div>
      <div class="table-corner">
        <b-table id="my-table"  stacked="md" :no-local-sorting="true" head-variant="light" v-bind="$attrs" small>
          <template #top-row>
              <slot name="filter"></slot>
          </template>
          <template v-slot:table-busys>
            <div class="text-center my-2">
              <b-spinner class="align-middle text-primary"></b-spinner>
              <strong class="ml-2">Loading...</strong>
            </div>
          </template>
          <template v-for="(index, name) in $scopedSlots" v-slot:[name]="data">
              <slot :name="name" v-bind="data"></slot>
          </template>
          <template v-slot:cell(actions)="row" >
            <div class="div-actions">
              <span v-if="showActionButton">
                  <b-overlay
                      rounded
                      opacity="0.6"
                      spinner-small
                      spinner-variant="primary"
                      class="d-inline-block"
                  >
                  <click-confirm>
                      <b-button size="sm" variant="danger" @click="deleteRowAction(row)" class="mr-2 btn-xs">
                      <b-icon-trash></b-icon-trash>
                      </b-button>
                  </click-confirm>
                  </b-overlay>
              </span>
            </div>
          </template>
        </b-table>
      </div>
      <b-row>
        <b-col cols="12" lg="8">
            <b-button v-if="showAddButton" variant="success" size="sm" @click="showPopupAddAction"><b-icon-plus></b-icon-plus> Tambah</b-button>
            <b-button v-if="showAddFromClipboardButton" variant="primary" class="ml-2" size="sm"  @click="addFromClipboardAction"><b-icon-pencil></b-icon-pencil> Add From Clipboard</b-button>
            <b-button v-if="showReplaceFromClipboardButton" variant="primary" class="ml-sm-2 mt-2 mt-sm-0" size="sm" @click="replaceFromClipboardAction"><b-icon-pencil></b-icon-pencil> Replace From Clipboard</b-button>
            <b-overlay 
              :show="isDownloadBusy"
              rounded
              opacity="0.6"
              spinner-small
              spinner-variant="primary"
              class="d-inline-block">
            <b-button v-if="showDownloadButton" variant="success" class="ml-2 mt-2 mt-sm-0" size="sm" @click="downloadAction"><b-icon-download></b-icon-download> Download</b-button>
            </b-overlay>
            <b-button v-if="showUploadButton" variant="warning" class="ml-2 mt-2 mt-sm-0" size="sm" @click="showPopupUploadAction"><b-icon-upload></b-icon-upload> Upload</b-button>
        </b-col>
        <b-col cols="12" lg="4">
            <div class="float-lg-right mt-3 mt-lg-0">
              <slot name="table-paging"></slot>
            </div>
        </b-col>
      </b-row>
      <b-row>
         <b-col cols="12">
            <slot name="form-add-detail"></slot>
        </b-col>
      </b-row>
      <b-row>
         <b-col cols="12">
            <slot name="upload-detail"></slot>
        </b-col>
      </b-row>
    </div>
</template>
<style lang="scss" scoped>
  /deep/.b-table-top-row {
    background-color: #f1f1f1!important;
  }
</style>
<script>
export default {
    props: ["options", "showActionButton", "showSelectAll", "isDataLoading", "isDownloadBusy", "showAddButton", "showAddFromClipboardButton", "showReplaceFromClipboardButton", "showDownloadButton", "showUploadButton", "showPaging"],
    data() {
        return {
            
        }
    },
    mounted() {
      
    },
    methods: {
        setSelected(value) {
            this.inputModel = value;
        },
        addButtonAction() {
          this.$emit("addButtonMethod");
        },
        uploadButtonAction(modalEvent) {
          modalEvent.preventDefault();
          this.$emit("uploadButtonMethod", modalEvent);
        },
        editRowAction(item) {
          console.log(item);
          let itemId = item.item.id;
          this.$emit("editButtonMethod", itemId);
        },
        deleteRowAction(item) {
          let itemId = item.item.id;
          this.$emit("deleteButtonMethod", itemId);
        },
        showPopupAddAction() {
          this.$emit("showPopupAddMethod");
        },
        showPopupUploadAction() {
          this.$emit("showPopupUploadMethod");
        },
        addFromClipboardAction() {
          this.$emit("addFromClipboardMethod");
        },
        replaceFromClipboardAction() {
          this.$emit("replaceFromClipboardMethod");
        },
        downloadAction() {
          this.$emit("downloadMethod");
        }
    }
}
</script>
<style lang="scss" scoped>
/deep/ .modal-body {
  background: #EEF0F8 !important;
}
@media (min-width: 768px) {
  /deep/ .modal-detail {
      width: 50% !important;
      height: 80% !important;
      margin-top: 4.75rem !important;
  } 
}
</style>