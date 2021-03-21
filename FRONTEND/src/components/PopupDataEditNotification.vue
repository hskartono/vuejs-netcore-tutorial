<template>
<div>
    <b-modal id="my-modal" scrollable 
        :no-close-on-backdrop=true 
        :title="label" 
        :no-close-on-esc=true 
        v-model="isShowPopup" 
        ok-title="Ok" 
        ok-variant="success" 
        :ok-only="true"
        @hidden="resetModal"
    >
    <b-card v-for="item in result" :key="item.documentId" class="mt-2">
      <div>{{ item.message }}</div>
      <div>Document ID : {{ item.documentId }}</div>
      <div>
        <span class="float-right">
          <b-overlay 
              :show="isDeleteData"
              rounded
              opacity="0.6"
              spinner-small
              spinner-variant="primary"
              class="d-inline-block">
              <click-confirm>
          <b-button variant="danger" size="sm" @click="discardData(item.documentId)">Discard</b-button>
          </click-confirm>
          </b-overlay>
          <b-button variant="success" size="sm" @click="editData(item.documentId)" class="ml-2">Edit</b-button>
        </span>
      </div>
    </b-card>
    </b-modal>
</div>
</template>
<style lang="scss" scoped>
  /deep/ .modal-dialog {
      max-width: 600px !important;
      max-height: 500px !important;
      margin-top: 4.75rem !important;
  }
</style>
<script>
import axios from 'axios';
import { saveAs } from 'file-saver';

export default {
    props: ["entityName", "label"],
    data() {
      return {
        progressInterval : null,
        isBusy : false,
        isSuccess : false,
        dataUrl : "",
        fileName : "",
        isShowPopup: false,
        result: [],
        isDeleteData: false,
      };
    },
    mounted() {
      this.checkEditor();
    },
    methods: {
        resetModal() {
          this.isShowPopup = false;
          this.$emit('resetModalMethod');
        },
        async discardData(id) {
          this.isDeleteData = true;
          this.$http.delete(process.env.VUE_APP_API_URL + this.entityName + '/discard/' + id)
          .then(response => {
            this.isDeleteData = false;
            this.checkEditor();
          }).catch(error => {
            this.isDeleteData = false;
            this.checkEditor();
          });
        },
        editData(id) {
          this.isShowPopup = false;
          this.$router.push({ name: this.entityName + 'edit', params : { id: id } });
        },
        async checkEditor() {
          this.isBusy = true;
          this.$http.get(process.env.VUE_APP_API_URL + this.entityName + '/draftlist')
          .then(response => {
            this.result = [];
            if (response != null) {
                if (response.data.length > 0) {
                  this.isShowPopup = true;
                  this.result = response.data;
                }
            }
          }).catch(error => {});
        },
    }
}
</script>